using Microsoft.Azure.Functions.Worker;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
//using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamics_AI_Function_App
{
    public class CRM_Tools
    {
       public static ILogger<Function1> _logger { get; set; }
        private static IOrganizationService _organizationService { get; set; }

        private static string organizationUri="https://orge54d366e.crm8.dynamics.com/";
        //private static string organizationUri= "https://orgca8cd069.crm8.dynamics.com/";
        private static void Connect()
        {
            //var organizationUri = "https://orgca8cd069.crm8.dynamics.com/";




            
           
            var service1 = new ServiceClient(new Uri(organizationUri), getAccessToken, true);
            _organizationService = (IOrganizationService)service1;
        }

        private static async Task<string> getAccessToken(string ss)
        {
            //var organizationUri_ = "https://orgca8cd069.crm8.dynamics.com/";
            //organizationUri = "https://orge54d366e.crm8.dynamics.com/";

            

            var client = "<clint-id>"; //
            var clientCode= "<>"; //

            string[] scopes = { $"{organizationUri}/.default" };



            var confBuilder = ConfidentialClientApplicationBuilder.Create(client)
                              .WithClientSecret(clientCode)
                              .WithAuthority($"https://login.microsoftonline.com/911642f3-f0ea-4d16-ba0b-f7c52e6ee9b8/oauth2/v2.0/token")
                              .Build();


            var authres = await confBuilder.AcquireTokenForClient(scopes)
                .ExecuteAsync();
            return (string)authres.AccessToken;
        }

        //[Description("Get CRM Data and count of list of records")]
        public static List<Dictionary<string, object>> GetDatafromDynamicsCRM(string fetchXml)
        {
            var list = new List<Dictionary<string, object>>();

            try
            {
                if (_organizationService == null)
                {
                    _logger.LogInformation("Connected to dynamics CRM successfully.");
                    Connect();
                }

                _logger.LogInformation("GetDatafromDynamicsCRM.");
                var results = _organizationService?.RetrieveMultiple(new FetchExpression(fetchXml));

                foreach (var entity in results.Entities)
                {
                    var dict = new Dictionary<string, object>();

                    foreach (var attr in entity.Attributes)
                    {
                        dict[attr.Key] = attr.Value;
                    }

                    list.Add(dict);
                }
            }
            catch (Exception ex)
            {
                var dict = new Dictionary<string, object>();
                dict["error"] = ex.Message;
                list.Add(dict);
            }
             return list;
        }

        public static async Task<string> CreateActivity(CreateActivityInput input)
        {
            try
            {
                if (_organizationService == null)
                {
                    _logger.LogInformation("Connected to dynamics CRM successfully.");

                    Connect();
                }

                _logger.LogInformation("Creating an Activity..");

                Entity activity = new Entity(input.ActivityType);

                // Common fields
                activity["subject"] = input.Subject ?? "No Subject";
                if (!string.IsNullOrEmpty(input.Description))
                    activity["description"] = input.Description;

                if (!string.IsNullOrEmpty(input.RegardingEntityLogicalName) && input.RegardingEntityId != Guid.Empty)
                    activity["regardingobjectid"] = new EntityReference(input.RegardingEntityLogicalName, input.RegardingEntityId);

                if (input.OwnerId.HasValue)
                    activity["ownerid"] = new EntityReference("systemuser", input.OwnerId.Value);

                if (input.ScheduledStart.HasValue)
                    activity["scheduledstart"] = input.ScheduledStart.Value;
                if (input.ScheduledEnd.HasValue)
                    activity["scheduledend"] = input.ScheduledEnd.Value;
                if (input.PriorityCode.HasValue)
                    activity["prioritycode"] = new OptionSetValue(input.PriorityCode.Value);

                // Add recipients (ActivityParty)
                if (input.To.Any())
                    activity["to"] = CreateActivityPartyList(input.To);// input.To.Select(p => new EntityReference(p.EntityLogicalName, p.EntityId)).ToArray();
                if (input.From.Any())
                    activity["from"] = CreateActivityPartyList(input.From); //input.From.Select(p => new EntityReference(p.EntityLogicalName, p.EntityId)).ToArray();
                if (input.CC.Any())
                    activity["cc"] = CreateActivityPartyList(input.CC); //input.CC.Select(p => new EntityReference(p.EntityLogicalName, p.EntityId)).ToArray();
                if (input.BCC.Any())
                    activity["bcc"] = CreateActivityPartyList(input.BCC); //input.BCC.Select(p => new EntityReference(p.EntityLogicalName, p.EntityId)).ToArray();

                Guid activityId = _organizationService.Create(activity);

                return $"Activity created successfully with ID: {activityId}";
            }
            catch (Exception ex)
            {
                return $"Failed to create activity: {ex.Message}";
            }
        }

        private static Entity[] CreateActivityPartyList(List<ActivityPartyInput> partyInputs)
        {
            return partyInputs.Select(p => new Entity("activityparty")
            {
                ["partyid"] = new EntityReference(p.EntityLogicalName, p.EntityId)
            }).ToArray();
        }
        public static void Create_record_In_CRM(Entity record)
        {
            List<Entity> entities = new List<Entity>();
            entities.Add(record);

            foreach (var eachEntity in entities)
            {
                try
                {
                    var record_id = _organizationService.Create(eachEntity);

                }
                catch(Exception ex)
                {

                }
            }
        }


        
       public static string GetRetrieveRecordChangeHistory(string entityschemaname, string record_id)
        {
            string auditInfo = string.Empty;

            if (_organizationService == null)
            {
                _logger.LogInformation("Connected to dynamics CRM successfully.");

                Connect();
            }

            _logger.LogInformation("Get_Audit_Changes.");

            var req = new RetrieveRecordChangeHistoryRequest
            {
                Target = new EntityReference(entityschemaname,Guid.Parse(record_id)),
                PagingInfo = new PagingInfo
                {
                    PageNumber = 1,
                    Count = 20,
                    ReturnTotalRecordCount = true
                }
            };

            

            var resp = (RetrieveRecordChangeHistoryResponse)_organizationService.Execute(req);
            var auditDetailCollection = resp.AuditDetailCollection;

            int recordsReturned = auditDetailCollection.AuditDetails.Count;
            int totalRecords = auditDetailCollection.TotalRecordCount;
            auditInfo+=$"Retrieved {recordsReturned} of {totalRecords} auditdetail records.";

            auditDetailCollection.AuditDetails.ToList().ForEach(x =>
            {

                Entity auditRecord = x.AuditRecord;

            auditInfo += $"Change Date: {auditRecord.FormattedValues["createdon"]}";
                auditInfo += $"Change By: {((EntityReference)auditRecord["userid"]).Name}";
                auditInfo += $"Action: {auditRecord.FormattedValues["action"]}";
                auditInfo += $"Operation: {auditRecord.FormattedValues["operation"]}";



                //ShowAuditDetail(_organizationService, auditRecord.Id, auditInfo);
                auditInfo += DisplayAuditDetail(x)+" ";

                Console.WriteLine();
            });

            return auditInfo;
        }

        

        public static string GetTypedValueAsString(object typedValue)
        {

            string value = string.Empty;

            switch (typedValue)
            {
                case OptionSetValue o:
                    value = o.Value.ToString();
                    break;
                case EntityReference e:
                    value = $"LogicalName:{e.LogicalName},Id:{e.Id},Name:{e.Name}";
                    break;
                default:
                    value = typedValue.ToString();
                    break;
            }

            return value;

        }
        

        static string DisplayAuditDetail(AuditDetail auditDetail)
        {
            string auditInfo = "";
            switch (auditDetail)
            {

                case AttributeAuditDetail aad:

                    Entity oldRecord = aad.OldValue;
                    Entity newRecord = aad.NewValue;
                    List<string> oldKeys = new List<string>();

                    //Look for changed or deleted values that are included in the OldValue collection
                    oldRecord.Attributes.Keys.ToList().ForEach(k =>
                    {
                        if (oldRecord.FormattedValues.Keys.Contains(k))
                        {
                            if (newRecord.FormattedValues.Contains(k))
                            {
                                auditInfo +=
                                    $"\tChange:{k}:{oldRecord.FormattedValues[k]} => " +
                                    $"{newRecord.FormattedValues[k]}";
                            }
                            else
                            {

                                auditInfo += $"\tDeleted:{k}:" +
                                    $"{oldRecord.FormattedValues[k]}";
                            }
                        }
                        else
                        {
                            if (newRecord.Attributes.Keys.Contains(k))
                            {

                                auditInfo += $"\tChange:{k}:{oldRecord[k]} => " +
                                    $"{newRecord[k]}";
                            }
                            else
                            {
                                auditInfo += $"\tDeleted:{k}:{oldRecord[k]}";
                            }
                        }

                        oldKeys.Add(k); //Add to list so we don't check again
                    });

                    //Look for New values that are only in the NewValues collection
                    newRecord.Attributes.Keys.ToList().ForEach(k =>
                    {
                        if (!oldKeys.Contains(k))//Exclude any keys for changed or deleted values
                        {
                            if (newRecord.FormattedValues.Keys.Contains(k))
                            {
                                auditInfo += $"\tNew Value:{k} => " +
                                    $"{newRecord.FormattedValues[k]}";
                            }
                            else
                            {
                                auditInfo += $"\tNew Value:{k}:{newRecord[k]}";
                            }
                        }
                    });
                    break;

                case ShareAuditDetail sad:
                    auditInfo += $"\tUser: {sad.Principal.Name}";
                    auditInfo += $"\tOld Privileges: {sad.OldPrivileges}";
                    auditInfo += $"\tNew Privileges: {sad.NewPrivileges}";
                    break;

                //Applies to operations on N:N relationships
                case RelationshipAuditDetail rad:
                    auditInfo += $"\tRelationship Name :{rad.RelationshipName}";
                    auditInfo += $"\tRecords:";
                    rad.TargetRecords.ToList().ForEach(y =>
                    {
                        auditInfo += $"\tTarget Record :{y.Name}";
                    });
                    break;

                //Only applies to role record
                case RolePrivilegeAuditDetail rpad:

                    List<string> newRolePrivileges = new List<string>();
                    rpad.NewRolePrivileges.ToList().ForEach(y =>
                    {
                        if (y != null)
                        {
                            newRolePrivileges.Add(
                            $"\t\tPrivilege Id:{y.PrivilegeId} Depth:{y.Depth}\n");
                        }
                    });

                    List<string> oldRolePrivileges = new List<string>();
                    rpad.OldRolePrivileges.ToList().ForEach(y =>
                    {
                        if (y != null)
                        {
                            oldRolePrivileges.Add(
                            $"\t\tPrivilege Id:{(y.PrivilegeId)} Depth:{y.Depth}\n");
                        }
                    });

                    List<string> invalidNewPrivileges = new List<string>();
                    rpad.InvalidNewPrivileges.ToList().ForEach(y =>
                    {
                        if (y != null)
                        {
                            invalidNewPrivileges.Add(
                            $"\t\tGuid:{y}\n");
                        }
                    });

                    auditInfo += $"\tNew Role Privileges:\n{string.Join(string.Empty, newRolePrivileges.ToArray())}";
                    auditInfo += $"\tOld Role Privileges:\n{string.Join(string.Empty, oldRolePrivileges.ToArray())}";
                    auditInfo += $"\tInvalid New Privileges:\n{string.Join(string.Empty, invalidNewPrivileges.ToArray())}"; ;
                    break;

                //Only applies for systemuser record
                case UserAccessAuditDetail uaad:
                    auditInfo += $"\tAccess Time:{uaad.AccessTime}";
                    auditInfo += $"\tInterval:{uaad.Interval}";
                    break;

            }

            return auditInfo;
        }




        public static string Get_Audit_Changes(string entityschemaname, string record_id)
        {
            // record reference for which you want audit history
            string changes = "";
            try
            {
                if (_organizationService == null)
                {
                    _logger.LogInformation("Connected to dynamics CRM successfully.");

                    Connect();
                }

                _logger.LogInformation("Get_Audit_Changes.");

                var target = new EntityReference(entityschemaname, new Guid(record_id));

                var query = new QueryExpression("audit")
                {
                    NoLock = true,
                    ColumnSet = new ColumnSet(
                        "action",
                        "actionname",
                        "createdon",
                        "operation",
                        "operationname",
                        "userid",
                        "oldvalue",
                        "newvalue",
                        "objecttypecode",
                        "attributemask"
                    )
                };

                query.Criteria.AddCondition("objectid", ConditionOperator.Equal, target.Id);
                query.Orders.Add(new OrderExpression("createdon", OrderType.Descending));

                var audits = _organizationService.RetrieveMultiple(query);

                foreach (var audit in audits.Entities)
                {
                    changes += string.Format($"Date: {audit.GetAttributeValue<DateTime>("createdon")}");
                    changes += string.Format($"Operation: {audit.GetAttributeValue<OptionSetValue>("operation")?.Value}");
                    changes += string.Format($"Action: {audit.GetAttributeValue<OptionSetValue>("action")?.Value}");
                }

                foreach (var audit in audits.Entities)
                {
                    var auditId = audit.Id;

                    var detailReq = new RetrieveAuditDetailsRequest
                    {
                        AuditId = auditId
                    };

                    var detailResp = (RetrieveAuditDetailsResponse)_organizationService.Execute(detailReq);

                    var detail = detailResp.AuditDetail;

                    // Attribute Values Changed
                    if (detail is AttributeAuditDetail attr)
                    {
                        Console.WriteLine("\n--- Attribute Changes ---");

                        foreach (var key in attr.NewValue.Attributes.Keys)
                        {
                            var newValue = attr.NewValue[key]?.ToString();
                            attr.OldValue.TryGetAttributeValue<Entity>(key, out var oldValue);

                            changes += string.Format($"{key}: {oldValue} ➜ {newValue}");
                        }
                    }

                    // Relationship changes (Associate / Disassociate)
                    if (detail is RelationshipAuditDetail rel)
                    {
                        Console.WriteLine("\n--- Relationship Changes ---");
                        //Console.WriteLine($"Relationship: {rel.Relationship.Name}");

                        foreach (var record in rel.TargetRecords)
                            changes += string.Format($"Record: {record.Id}");
                    }

                    // Attribute mask audit (changed fields)
                    if (detail is AttributeAuditDetail)
                    {
                        changes += string.Format("\nChanged Fields:");
                        foreach (var attrName in ((AttributeAuditDetail)detail).NewValue.Attributes.Keys)
                            changes += string.Format($"- {attrName}");
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return changes;
        }
        public static string GetRecordCountfromDynamicsCRM(string fetchXml)
        {
            string record_count = "0";
            try
            {
                if (_organizationService == null)
                {
                    Connect();
                }

                _logger.LogInformation("Connected to dynamics CRM successfully.");
                var results = _organizationService?.RetrieveMultiple(new FetchExpression(fetchXml));
                record_count = results?.Entities.Count().ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return record_count;
        }

    }
}
