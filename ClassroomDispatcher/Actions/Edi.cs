using System;
using ClassroomDispatcher.Extensions;

namespace ClassroomDispatcher.Actions
{
    [Label("Drill EDI")]
    internal class Edi : Dispatcher.SqlDispatchable
    {
        public Edi(string connectionString)
        : base(connectionString)
        { }

        public override void Execute(int iterations, int interval, byte etape = 1)
        {
            if (!ShowOk()) return;

            var qry = @"IF OBJECT_ID(N'dbo.message_edi', N'U') IS NOT NULL DROP TABLE dbo.message_edi;";

            if (etape == 1)
            {
                // création de la table
                Servant.ExecuteNonQuery(qry);

                qry = @"CREATE TABLE dbo.message_edi (
                            message_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT(NEWID()),
	                        date_message datetime2(2) NOT NULL DEFAULT(SYSDATETIME()),
                            contenu text
                            );";
                Servant.ExecuteNonQuery(qry);

                // première alimentation
                qry = @"INSERT INTO dbo.message_edi
                            (contenu)
                            SELECT text
                            FROM master.sys.messages v1
                            CROSS JOIN (VALUES(1), (2)) v2(c);";
                Servant.ExecuteNonQuery(qry);

                // trigger
                qry = @"CREATE TRIGGER atr_message_edi
                        ON dbo.message_edi
                        AFTER INSERT
                        AS BEGIN
                            IF EXISTS(
                                SELECT m.message_id
                                FROM dbo.message_edi m
                                JOIN inserted i ON m.date_message > i.date_message
                            )
                            BEGIN
                                UPDATE m
                                SET date_message = (SELECT MAX(date_message) FROM dbo.message_edi)
		                        FROM dbo.message_edi m
                                JOIN inserted i ON m.message_id = i.message_id
                            END
                        END;";
                Servant.ExecuteNonQuery(qry);
            }
            // alimentation en boucle
            for (var i = 0; i <= iterations; i++)
            {
                qry = String.Format("INSERT INTO dbo.message_edi (contenu) VALUES ('{0}');", Guid.NewGuid());
                Servant.ExecuteNonQuery(qry);
                System.Threading.Thread.Sleep(interval);
            }
        }

    }
}
