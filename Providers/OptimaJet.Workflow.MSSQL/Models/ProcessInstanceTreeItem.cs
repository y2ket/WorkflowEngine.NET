using System;
using System.Collections.Generic;
using System.Data;
#if NETCOREAPP
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptimaJet.Workflow.Core.Model;
using OptimaJet.Workflow.Core.Persistence;

// ReSharper disable once CheckNamespace

namespace OptimaJet.Workflow.DbPersistence
{
    public class ProcessInstanceTreeItem : DbObject<ProcessInstanceTreeItem>, IProcessInstanceTreeItem
    {
        static ProcessInstanceTreeItem()
        {
            DbTableName = "virtual"; //this entity is combination of WorkflowProcessInstance and WorkflowProcessScheme
        }

        public ProcessInstanceTreeItem()
        {
            DBColumns.AddRange(new[]
            {
                new ColumnInfo {Name = "Id", IsKey = true, Type = SqlDbType.UniqueIdentifier},
                new ColumnInfo {Name = "ParentProcessId", Type = SqlDbType.UniqueIdentifier},
                new ColumnInfo {Name = "RootProcessId", Type = SqlDbType.UniqueIdentifier}, 
                new ColumnInfo {Name = "StartingTransition"},
                new ColumnInfo {Name = nameof(SubprocessName)},
            });
        }

        public Guid Id { get; set; }
        public Guid? ParentProcessId { get; set; }
        public Guid RootProcessId { get; set; }
        public string StartingTransition { get; set; }
        public string SubprocessName { get; set; }

        public override object GetValue(string key)
        {
            switch (key)
            {
                case "Id":
                    return Id;
                case "ParentProcessId":
                    return ParentProcessId;
                case "RootProcessId":
                    return RootProcessId;
                case "StartingTransition":
                    return StartingTransition;
                case nameof(SubprocessName):
                    return SubprocessName;
                default:
                    throw new Exception($"Column {key} is not exists");
            }
        }

        public override void SetValue(string key, object value)
        {
            switch (key)
            {
                case "Id":
                    Id = (Guid)value;
                    break;
                case "ParentProcessId":
                    ParentProcessId = value as Guid?;
                    break;
                case "RootProcessId":
                    RootProcessId = (Guid)value;
                    break;
                case "StartingTransition":
                    StartingTransition = value as string;
                    break;
                case nameof(SubprocessName):
                    SubprocessName = value as string;
                    break;
                default:
                    throw new Exception(string.Format("Column {0} is not exists", key));
            }
        }


        public static async Task<List<IProcessInstanceTreeItem>> GetProcessTreeItemsByRootProcessIdAsync(SqlConnection connection, Guid rootProcessId)
        {
            var builder = new StringBuilder();
            builder.Append("SELECT ");
            builder.Append($"[{nameof(Id)}], [{nameof(ParentProcessId)}], [{nameof(RootProcessId)}], [{nameof(StartingTransition)}], [{nameof(SubprocessName)}] ");
            builder.Append($"FROM {WorkflowProcessInstance.ObjectName} ");
            builder.Append($"WHERE [{nameof(RootProcessId)}] = @rootProcessId");


            return (await SelectAsync(connection, builder.ToString(), new SqlParameter("rootProcessId", SqlDbType.UniqueIdentifier) {Value = rootProcessId})
                .ConfigureAwait(false)).Cast<IProcessInstanceTreeItem>().ToList();
        }
    }
}
