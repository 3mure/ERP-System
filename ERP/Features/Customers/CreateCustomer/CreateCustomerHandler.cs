using BuildingBlocks.Interfaces;
using Catalog_Service.Entities;
using MediatR;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;

namespace Catalog_Service.Features.Customers.CreateCustomer
{
    public class CreateCustomerHandler
    : IRequestHandler<CreateCustomerCommand, CreateCustomerResult>
    {
        private readonly IBaseRepository<Customer> _customersRepo;
        private readonly IBaseRepository<CustomerGroup> _groupsRepo;
        private readonly IBaseRepository<User> _usersRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CreateCustomerHandler(
            IBaseRepository<Customer> customersRepo,
            IBaseRepository<CustomerGroup> groupsRepo,
            IBaseRepository<User> usersRepo,
            IUnitOfWork unitOfWork)
        {
            _customersRepo = customersRepo;
            _groupsRepo = groupsRepo;
            _usersRepo = usersRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateCustomerResult> Handle(
            CreateCustomerCommand command,
            CancellationToken cancellationToken)
        {
            var req = command.Request;
            var basic = req.BasicInformation;

            // ── Guard 1: Phone already exists ────────────────────
            var phoneExists = await _customersRepo
                .Get(c => c.Phone == basic.PhoneNumber && !c.IsDeleted)
                .AnyAsync(cancellationToken);

            if (phoneExists)
                return new CreateCustomerResult(
                    Success: false,
                    Message: "Customer already exists.");

            // ── Resolve CustomerGroup (optional) ─────────────────
            int? groupId = null;
            if (!string.IsNullOrWhiteSpace(req.AccountDetails?.CustomerGroup))
            {
                var group = await _groupsRepo
                    .Get(g => g.Name == req.AccountDetails.CustomerGroup && !g.IsDeleted)
                    .FirstOrDefaultAsync(cancellationToken);

                groupId = group?.Id;
            }

            // ── Resolve RegisteredBy employee (optional) ──────────
            int? registeredById = null;
            if (!string.IsNullOrWhiteSpace(req.AccountDetails?.RegisteredBy))
            {
                var employee = await _usersRepo
                    .Get(u => u.Name == req.AccountDetails.RegisteredBy && !u.IsDeleted)
                    .FirstOrDefaultAsync(cancellationToken);

                registeredById = employee?.Id;
            }

            // ── Hash password if provided ─────────────────────────
            string? passwordHash = null;
            if (!string.IsNullOrWhiteSpace(basic.Password))
            {
                // Generate a salt
                var salt = new byte[16];
                using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }
                var passwordBytes = BCrypt.PasswordToByteArray(basic.Password.ToCharArray());
                var hashBytes = BCrypt.Generate(passwordBytes, salt, 10); // 10 is a typical cost factor
                passwordHash = Convert.ToBase64String(hashBytes);
            }

            // ── Build entity ──────────────────────────────────────
            var customer = new Customer
            {
                NameEn = basic.NameEn,
                Email = basic.EmailAddress,
                Phone = basic.PhoneNumber,
                PasswordHash = passwordHash,
                Type = basic.Type ?? "Individual",
                CustomerGroupId = groupId,
                ReferralSource = req.AccountDetails?.ReferralSource,
                RegisteredBy = registeredById,
                Area = req.LocationAndCustom?.Area,
                FullAddress = req.LocationAndCustom?.FullAddress,
                Industry = req.LocationAndCustom?.Industry,
                LoyaltyPoints = 0,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _customersRepo.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateCustomerResult(
                Success: true,
                Message: "Customer created successfully.",
                CustomerId: customer.Id);
        }
    }
}
