﻿using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.PluginTelemetry;
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
        private readonly ILogger _logger;

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
                                interestEntity["sl_interestlistid"] = new EntityReference("sl_interestlist", interestlistId);
                                interestEntity["sl_contactid"] = new EntityReference("contact", contactId);
                                _service.Create(interestEntity);
                            }
                        }

                        if (optionSetValues.Count > 0)
                            contactEntity.Attributes["sl_interest"] = optionSetValues;
                    }

                    return contactId;
                }

                return new Guid();
            }
            catch (Exception ex)
            {
                _logger.LogError($"InterestService:AddContact:- {ex}");
                return new Guid();
            }
        }
    }
}