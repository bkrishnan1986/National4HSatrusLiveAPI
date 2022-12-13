using log4net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using National4HSatrusLive.Models;
using System;
using System.Collections.Generic;

namespace National4HSatrusLive.Services
{
    public class InterestService
    {
        private OrganizationServiceProxy _service;

        /// <summary>
        /// The logger
        /// </summary>
        private ILog Logger = LogManager.GetLogger(typeof(ContactService));

        public InterestService()
        {
            _service = OrganizationService.GetCrmService();
        }

        public Guid AddInterest(ContactModel contactModel, Guid contactId)
        {
            try
            {
                if (contactModel != null && !string.IsNullOrWhiteSpace(contactModel.FirstName))
                {
                    Entity contactEntity = new Entity("contact");

                    if (contactModel.Interests != null && contactModel.Interests.Count > 0)
                    {

                        List<OptionSetValue> optionSetValues = new List<OptionSetValue>();
                        foreach (var interest in contactModel.Interests)
                        {
                            var interestquery = new QueryExpression("sl_interestlist");
                            interestquery.ColumnSet = new ColumnSet(true);
                            interestquery.Criteria.AddCondition(new ConditionExpression("sl_name", ConditionOperator.Equal, interest));
                            var interestList = _service.RetrieveMultiple(interestquery);
                            if (interestList != null && interestList.Entities.Count > 0)
                            {
                                Entity interestEntity = new Entity("sl_interest");
                                var interestlistId = interestList.Entities[0].Id;
                                if (!isInterestExists(contactId, interestlistId))
                                {
                                    interestEntity["sl_interestlistid"] = new EntityReference("sl_interestlist", interestlistId);
                                    interestEntity["sl_contactid"] = new EntityReference("contact", contactId);
                                    _service.Create(interestEntity);
                                }
                            }
                        }

                    }

                    return contactId;
                }
                Logger.Info("returned new Guid() since new contact is null or first name is empty - InterestService.AddInterest");
                return new Guid();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new Guid();
            }
        }

        public bool isInterestExists(Guid contactId, Guid InterestListId)
        {
            try
            {
                var query = new QueryExpression("sl_interest");
                query.ColumnSet = new ColumnSet(true);
                query.Criteria.AddCondition(new ConditionExpression("sl_interestlistid", ConditionOperator.Equal, InterestListId));
                query.Criteria.AddCondition(new ConditionExpression("sl_contactid", ConditionOperator.Equal, contactId));

                var interestLists = _service.RetrieveMultiple(query);
                if (interestLists != null && interestLists.Entities.Count > 0)
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