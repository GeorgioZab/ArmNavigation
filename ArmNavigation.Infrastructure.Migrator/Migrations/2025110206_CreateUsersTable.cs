using FluentMigrator;

namespace ArmNavigation.Infrastructure.Migrator.Migrations
{
    [Migration(2025110206)]
    public class CreateUsersTable : Migration
    {
        public override void Up()
        {
            // TODO медорганизации не удаляются каскадно, дописать это в табличку с медорганизацией
            Execute.Sql("""
                     CREATE TABLE "Users" ("UserId" UUID PRIMARY KEY,
                     "Login" text not null UNIQUE,
                     "Password" text not null,
                     "IsRemoved" bool default false not null,
                     "Role" int not null,
                     "MedInstitutionId" UUID not null,

                     CONSTRAINT fk_MedInstitutions_Users FOREIGN KEY ("MedInstitutionId") REFERENCES "MedInstitutions" ("MedInstitutionId"));
                     """);
        }

        public override void Down()
        {
            Execute.Sql("""
                    DROP TABLE "Users"
                    """);
        }
    }
}
