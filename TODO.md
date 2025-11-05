# TODO: Refactor Role to int throughout the project

## Tasks
- [x] Update User model: Change Role from enum to int
- [x] Update migrations: Remove UserRoleId from Users table, add Role int column
- [x] Update UserRepository: Change queries to use Role int instead of UserRoleId and RoleName
- [x] Update JwtTokenService: Change GenerateToken to accept int role
- [x] Update JwtAuthService: Pass (int)user.Role to GenerateToken
- [x] Update all service interfaces and implementations: Change Role to int in parameters
- [x] Update all controllers: Change GetContext to return int role, update request/response models if needed
- [x] Update MedInstitutionsController: Keep GetRoleFromUser as is (already returns int)
- [x] Test the changes to ensure functionality remains intact
