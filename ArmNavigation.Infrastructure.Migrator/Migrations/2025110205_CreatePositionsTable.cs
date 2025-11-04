using FluentMigrator;

namespace ArmNavigation.Infrastructure.Migrator.Migrations
{
    [Migration(2025110205)]
    public class CreatePositionsTable : Migration
    {
        public override void Up()
        {
            Execute.Sql("""
                     CREATE TABLE "Positions" ("PositionId" UUID PRIMARY KEY,
                     "Time" timestamp not null,
                     "Coordinates" point not null,
                     "CarId" UUID not null,
                     
                     CONSTRAINT fk_Cars_Positions FOREIGN KEY ("CarId") REFERENCES "Cars" ("CarId"));
                     """);
        }

        public override void Down()
        {
            Execute.Sql("""
                    DROP TABLE "Positions"
                    """);
        }
    }
}
