using FluentMigrator;

namespace ArmNavigation.Infrastructure.Migrator.Migrations
{
    [Migration(2025110204)]
    public class CreateCarsTable : Migration
    {
        public override void Up()
        {
            Execute.Sql("""
                     CREATE TABLE "Cars" ("CarId" UUID PRIMARY KEY,
                     "RegNum" text not null,
                     "Gps-tracker" text,
                     "IsRemoved" bool default false not null,
                     "MedInstitutionId" UUID not null,

                     CONSTRAINT fk_MedInstitutions_Cars FOREIGN KEY ("MedInstitutionId") REFERENCES "MedInstitutions" ("MedInstitutionId"));
                     """);
        }

        public override void Down()
        {
            Execute.Sql("""
                    DROP TABLE "Cars"
                    """);
        }
    }
}
