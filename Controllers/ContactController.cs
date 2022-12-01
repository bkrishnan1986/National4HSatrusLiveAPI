using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using National4HSatrusLive.Models;
using System;
using System.Web.Http;

namespace National4HSatrusLive.Controllers
{
    public class ContactController : ApiController
    {
        private OrganizationServiceProxy _service;
        public ContactController()
        {
            _service = WebApiConfig.GetCrmService();
        }
        
        // GET api/<controller>
        [HttpGet]
        public EntityCollection Retrieve()
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
                return new EntityCollection();
            }
        }

        // GET api/<controller>/5
        [HttpGet]
        public Entity GetById(Guid id)
        {
            try
            {
                var contact = _service.Retrieve("contact", id, new ColumnSet(true));
                return contact;
            }
            catch (Exception ex)
            {
                return new Entity();
            }
        }
        
        // GET api/<controller>/5
        [HttpGet]
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
                return new EntityCollection();
            }
        }

        [HttpGet]
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
                return new EntityCollection();
            }
        }
        
        [HttpGet]
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
                return new EntityCollection();
            }
        }
        
        [HttpGet]
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
                return new EntityCollection();
            }
        }

        // POST api/<controller>
        [HttpPost]
        public Guid Post([FromBody] ContactModel contactModel)
        {
            try
            {
                Entity contactEntity = new Entity("contact");
                EntityCollection emailEntity = RetrieveByEmailId(contactModel.Email);
                EntityCollection lastNameEntity = RetrieveByLastName(contactModel.LastName);
                EntityCollection firstNameEntity = RetrieveByFirstName(contactModel.FirstName);

                if(emailEntity.Entities.Count > 0 || lastNameEntity.Entities.Count > 0 || firstNameEntity.Entities.Count > 0)
                {
                    return new Guid();
                }

                var genderValue = GetOptionsSetValueByText(_service, "contact", "gendercode", contactModel.Gender);
                contactEntity.Attributes["gendercode"] = new OptionSetValue(genderValue);
                var prefix = GetOptionsSetValueByText(_service, "contact", "sl_honorific", contactModel.Prefix);
                contactEntity.Attributes["sl_honorific"] = new OptionSetValue(prefix);

                contactEntity.Attributes["firstname"] = contactModel.FirstName;
                contactEntity.Attributes["lastname"] = contactModel.LastName;
                contactEntity.Attributes["emailaddress1"] = contactModel.Email;
                contactEntity.Attributes["address1_line1"] = contactModel.Address1Street1;
                contactEntity.Attributes["address1_line1"] = contactModel.Address1Street2;
                contactEntity.Attributes["address1_line1"] = contactModel.Address1Street3;
                contactEntity.Attributes["address1_city"] = contactModel.Address1City1;
                contactEntity.Attributes["address1_stateorprovince"] = contactModel.Address1StateProvince;
                contactEntity.Attributes["address1_postalcode"] = contactModel.Address1ZipPostalCode;
                contactEntity.Attributes["donotbulkemail"] = contactModel.SendBulkEmail;
                var contactId = _service.Create(contactEntity);
                return contactId;
            }
            catch (Exception ex)
            {
                return new Guid();
            }
        }

        // PUT api/<controller>/5
        //[HttpPut]
        //public void Update([FromBody] Entity entity)
        //{
        //    try
        //    {
        //        _service.Update(entity);
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        //Todo

        [HttpPut]
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

            }
        }

        // Activate/Deactivate Contact
        [HttpPut]
        public void ActivateContact(Guid contactId)
        {
            try
            {
                var contact = GetById(contactId);
                var stateCode = (OptionSetValue)contact.Attributes["statecode"];
                var contactStatus = new ContactModel();
                if (stateCode != null)
                {
                    if(stateCode.Value == 0)
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

            }
        }

        // DELETE api/<controller>/5
        [HttpDelete]
        public void Delete(Guid contactId)
        {
            try
            {
                _service.Delete("contact", contactId);
            }
            catch (Exception ex)
            {

            }
        }

        public int GetOptionsSetValueByText(OrganizationServiceProxy xrmService, string entityName, string attributeName, string optionSetTextValue)
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
    }
}