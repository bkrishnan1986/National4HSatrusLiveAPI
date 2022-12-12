using log4net;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.PluginTelemetry;
using Microsoft.Xrm.Sdk.Query;
using National4HSatrusLive.Models;
using System;
using System.Linq;
using System.Net;

namespace National4HSatrusLive.Services
{
    public class ContactService
    {
        private OrganizationServiceProxy _service;

        ParticipationService _participationService;


        InterestService _InterestService;

        /// <summary>
        /// The logger
        /// </summary>
        private ILog Logger = LogManager.GetLogger(typeof(ContactService));

        public ContactService()
        {
            _service = OrganizationService.GetCrmService();
            _participationService = new ParticipationService();
            _InterestService = new InterestService();
        }

        #region Retrieve

        /// <summary>
        /// method to Retrieve Contact
        /// </summary>
        /// <returns></returns>
        public EntityCollection RetrieveContact()
        {
            try
            {
                QueryExpression query = new QueryExpression()
                {
                    EntityName = "contact",
                    ColumnSet = new ColumnSet(true),
                    Criteria = new FilterExpression()
                };
                var contacts = _service.RetrieveMultiple(query);
                return contacts;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new EntityCollection();
            }
        }

        /// <summary>
        /// method to Retrieve contact by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EntityCollection RetrieveContactById(Guid id)
        {
            try
            {
                var query = new QueryExpression("contact");
                query.ColumnSet = new ColumnSet(true);
                query.Criteria.AddCondition(new ConditionExpression("contactid", ConditionOperator.Equal, id));
                var contacts = _service.RetrieveMultiple(query);
                return contacts;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new EntityCollection();
            }
        }

        /// <summary>
        /// method to Retrieve contact by first name
        /// </summary>
        /// <param name="firstName"></param>
        /// <returns></returns>
        public EntityCollection RetrieveByFirstName(string firstName)
        {
            try
            {
                var query = new QueryExpression("contact");
                query.ColumnSet = new ColumnSet(true);
                query.Criteria.AddCondition(new ConditionExpression("firstname", ConditionOperator.Equal, firstName));
                var contacts = _service.RetrieveMultiple(query);
                return contacts;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new EntityCollection();
            }
        }

        /// <summary>
        /// method to Retrieve contact by last name
        /// </summary>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public EntityCollection RetrieveByLastName(string lastName)
        {
            try
            {
                var query = new QueryExpression("contact");
                query.ColumnSet = new ColumnSet(true);
                query.Criteria.AddCondition(new ConditionExpression("lastname", ConditionOperator.Equal, lastName));

                var contacts = _service.RetrieveMultiple(query);
                return contacts;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new EntityCollection();
            }
        }

        /// <summary>
        /// method to Retrieve contact by email Id
        /// </summary>
        /// <param name="emailId"></param>
        /// <returns></returns>
        public EntityCollection RetrieveByEmailId(string emailId)
        {
            try
            {
                var query = new QueryExpression("contact");
                query.ColumnSet = new ColumnSet(true);
                query.Criteria.AddCondition(new ConditionExpression("emailaddress1", ConditionOperator.Equal, emailId));

                var contacts = _service.RetrieveMultiple(query);
                return contacts;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new EntityCollection();
            }
        }

        /// <summary>
        /// method to Retrieve contact by Contact Number
        /// </summary>
        /// <param name="contactNumber"></param>
        /// <returns></returns>
        public EntityCollection RetrieveByContactNumber(string contactNumber)
        {
            try
            {
                var query = new QueryExpression("contact");
                query.ColumnSet = new ColumnSet(true);
                query.Criteria.AddCondition(new ConditionExpression("sl_contactnumber", ConditionOperator.Equal, contactNumber));

                var contacts = _service.RetrieveMultiple(query);
                return contacts;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new EntityCollection();
            }
        }

        #endregion

        #region Add

        /// <summary>
        /// method to Add Contact
        /// </summary>
        /// <param name="contactModel"></param>
        /// <returns></returns>
        public Guid AddContact(ContactModel contactModel)
        {
            try
            {
                if (contactModel != null && !string.IsNullOrWhiteSpace(contactModel.FirstName))
                {
                    Entity contactEntity = new Entity("contact");

                    var query = new QueryExpression("contact");
                    query.ColumnSet = new ColumnSet(true);
                    FilterExpression incidentChildFilterOR = query.Criteria.AddFilter(LogicalOperator.Or);
                    FilterExpression incidentChildFilterAND = query.Criteria.AddFilter(LogicalOperator.And);
                    incidentChildFilterOR.AddCondition(new ConditionExpression("emailaddress1", ConditionOperator.Equal, contactModel.Email));
                    incidentChildFilterOR.AddCondition(new ConditionExpression("emailaddress2", ConditionOperator.Equal, contactModel.Email));
                    incidentChildFilterOR.AddCondition(new ConditionExpression("emailaddress3", ConditionOperator.Equal, contactModel.Email));
                    incidentChildFilterAND.AddCondition(new ConditionExpression("statuscode", ConditionOperator.Equal, 1));


                    var queryResult = _service.RetrieveMultiple(query);

                    if (queryResult != null && queryResult.Entities.Count > 0)
                    {
                        _participationService.AddParticipation(queryResult.Entities[0].Id);
                        _InterestService.AddInterest(contactModel, queryResult.Entities[0].Id);
                        return new Guid();
                    }

                    var genderValue = GetOptionsSetValueByText(_service, "contact", "gendercode", contactModel.Gender);
                    if (genderValue != -1)
                        contactEntity.Attributes["gendercode"] = new OptionSetValue(genderValue);
                    var prefix = GetOptionsSetValueByText(_service, "contact", "sl_honorific", contactModel.Prefix);
                    if (prefix != -1)
                    {
                        contactEntity.Attributes["sl_honorific"] = new OptionSetValue(prefix);
                    }
                    else
                    {
                        prefix = GetOptionsSetValueByText(_service, "contact", "sl_honorific", contactModel.Prefix.TrimEnd(new char[] {'.'}));
                        if (prefix != -1)
                        {
                            contactEntity.Attributes["sl_honorific"] = new OptionSetValue(prefix);
                        }

                    }

                    var sourceValue = GetOptionsSetValueByText(_service, "contact", "n4h_source", "Bizzabo");
                    if (sourceValue != -1)
                        contactEntity.Attributes["n4h_source"] = new OptionSetValue(sourceValue);

                    if (!string.IsNullOrWhiteSpace(contactModel.FirstName))
                        contactEntity.Attributes["firstname"] = contactModel.FirstName;
                    if (!string.IsNullOrWhiteSpace(contactModel.LastName))
                        contactEntity.Attributes["lastname"] = contactModel.LastName;
                    if (!string.IsNullOrWhiteSpace(contactModel.Email))
                        contactEntity.Attributes["emailaddress1"] = contactModel.Email;
                    if (!string.IsNullOrWhiteSpace(contactModel.Address1Street1))
                        contactEntity.Attributes["address1_line1"] = contactModel.Address1Street1;
                    if (!string.IsNullOrWhiteSpace(contactModel.Address1Street2))
                        contactEntity.Attributes["address1_line2"] = contactModel.Address1Street2;
                    if (!string.IsNullOrWhiteSpace(contactModel.Address1Street3))
                        contactEntity.Attributes["address1_line3"] = contactModel.Address1Street3;
                    if (!string.IsNullOrWhiteSpace(contactModel.Address1City1))
                        contactEntity.Attributes["address1_city"] = contactModel.Address1City1;
                    if (!string.IsNullOrWhiteSpace(contactModel.Address1StateProvince))
                        contactEntity.Attributes["address1_stateorprovince"] = contactModel.Address1StateProvince;
                    if (!string.IsNullOrWhiteSpace(contactModel.Address1ZipPostalCode))
                        contactEntity.Attributes["address1_postalcode"] = contactModel.Address1ZipPostalCode;
                    if (contactModel.Birthday != null && DateTime.MinValue != contactModel.Birthday)
                        contactEntity.Attributes["birthdate"] = contactModel.Birthday;
                    contactEntity.Attributes["donotbulkemail"] = !contactModel.SendBulkEmail;
                    var contactId = _service.Create(contactEntity);

                    _participationService.AddParticipation(contactId);

                    return contactId;
                }

                return new Guid();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new Guid();
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// method to Update Status
        /// </summary>
        /// <param name="contactModel"></param>
        public void UpdateStatus(ContactModel contactModel)
        {
            try
            {
                var stateRequest = new SetStateRequest();
                stateRequest.EntityMoniker = new EntityReference(contactModel.EntityName, contactModel.Id);
                stateRequest.State = new OptionSetValue(contactModel.StateValue);
                stateRequest.Status = new OptionSetValue(contactModel.StatusValue);

                _service.Execute(stateRequest);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /// <summary>
        /// method to Activate or Deactivate Contact
        /// </summary>
        /// <param name="contactId"></param>
        public void ActivateDeactivateContact(Guid contactId)
        {
            try
            {
                var contact = RetrieveContactById(contactId);
                var contactStatus = new ContactModel();
                if (0 == contact.Entities.Count)
                {
                    return;
                }
                var stateCode = (OptionSetValue)contact.Entities[0].Attributes["statecode"];
                if (stateCode != null)
                {
                    if (stateCode.Value == 0)
                    {
                        contactStatus = new ContactModel()
                        {
                            EntityName = "contact",
                            Id = contactId,
                            StateValue = 1,
                            StatusValue = 2
                        };
                    }
                    else
                    {
                        contactStatus = new ContactModel()
                        {
                            EntityName = "contact",
                            Id = contactId,
                            StateValue = 0,
                            StatusValue = 1
                        };
                    }
                }
                UpdateStatus(contactStatus);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// method to Delete Contact
        /// </summary>
        /// <param name="contactId"></param>
        public void DeleteContact(Guid contactId)
        {
            try
            {
                _service.Delete("contact", contactId);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// method to Get option set values by text
        /// </summary>
        /// <param name="xrmService"></param>
        /// <param name="entityName"></param>
        /// <param name="attributeName"></param>
        /// <param name="optionSetTextValue"></param>
        /// <returns></returns>
        private int GetOptionsSetValueByText(OrganizationServiceProxy xrmService, string entityName, string attributeName, string optionSetTextValue)
        {
            try
            {
                RetrieveAttributeRequest retrieveAttributeRequest = new RetrieveAttributeRequest
                {
                    EntityLogicalName = entityName,
                    LogicalName = attributeName,
                    RetrieveAsIfPublished = true
                };
                RetrieveAttributeResponse retrieveAttributeResponse = (RetrieveAttributeResponse)xrmService.Execute(retrieveAttributeRequest);
                if (retrieveAttributeResponse != null)
                {
                    PicklistAttributeMetadata optionsetAttributeMetadata = retrieveAttributeResponse.AttributeMetadata as PicklistAttributeMetadata;
                    if (optionsetAttributeMetadata != null)
                    {
                        OptionMetadata[] optionsetList = optionsetAttributeMetadata.OptionSet.Options.ToArray();
                        foreach (OptionMetadata optionsetMetaData in optionsetList)
                        {
                            if (optionsetMetaData.Label.UserLocalizedLabel.Label == optionSetTextValue)
                            {
                                return optionsetMetaData.Value.Value;
                            }
                        }
                    }
                }
                return -1;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return -1;
            }
        }

        #endregion
    }
}