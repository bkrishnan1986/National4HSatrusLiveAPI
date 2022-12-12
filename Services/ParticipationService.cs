using System;
using System.Collections.Generic;
using log4net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using National4HSatrusLive.Models;

namespace National4HSatrusLive.Services
{
    public class ParticipationService
    {
        private OrganizationServiceProxy _service;


        /// <summary>
        /// The logger
        /// </summary>
        private ILog Logger = LogManager.GetLogger(typeof(ContactService));

        public ParticipationService()
        {
            _service = OrganizationService.GetCrmService();
        }

        public Guid AddParticipation(Guid contactId)
        {
            try
            {
                var participationquery = new QueryExpression("sl_participationtype");
                participationquery.ColumnSet = new ColumnSet(true);
                participationquery.Criteria.AddCondition(new ConditionExpression("sl_name", ConditionOperator.Equal, "Ignite 2023"));
                var participationType = _service.RetrieveMultiple(participationquery);
                if (participationType != null && participationType.Entities.Count > 0)
                {
                    Entity participationEntity = new Entity("sl_participation");
                    var participationTypeId = participationType.Entities[0].Id;
                    if (!isParticipationExists(contactId, participationTypeId))
                    {
                        participationEntity["sl_participationtypeid"] = new EntityReference("sl_participationtype", participationTypeId);
                        participationEntity["sl_contactid"] = new EntityReference("contact", contactId);
                        _service.Create(participationEntity);
                    }
                }

                return contactId;

               // return new Guid();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new Guid();
            }
        }

        public bool isParticipationExists(Guid contactId, Guid participationTypeId)
        {
            try
            {
                var query = new QueryExpression("sl_participation");
                query.ColumnSet = new ColumnSet(true);
                query.Criteria.AddCondition(new ConditionExpression("sl_participationtypeid", ConditionOperator.Equal, participationTypeId));
                query.Criteria.AddCondition(new ConditionExpression("sl_contactid", ConditionOperator.Equal, contactId));

                var participationList = _service.RetrieveMultiple(query);
                if (participationList != null && participationList.Entities.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }
    }
}