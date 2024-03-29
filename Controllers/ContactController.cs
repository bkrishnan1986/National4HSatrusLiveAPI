﻿using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.PluginTelemetry;
using Microsoft.Xrm.Sdk.Query;
using National4HSatrusLive.Models;
using National4HSatrusLive.Services;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace National4HSatrusLive.Controllers
{
    [Authorize]
    public class ContactController : ApiController
    {
        /// <summary>
        /// The contact service
        /// </summary>
        private readonly ContactService _contactService;

        ParticipationService _participationService;
        /// <summary>
        /// The event service
        /// </summary>
        private readonly EventService _eventService;

        public ContactController()
        {
            _contactService = new ContactService();
            _eventService = new EventService();
            _participationService = new ParticipationService();
        }

        #region Retrieve Contact

        // GET api/<controller>
        /// <summary>
        /// method to Retrieve Contact
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public EntityCollection RetrieveContact()
        {
            try
            {
                var contacts = _contactService.RetrieveContact();
                return contacts;
            }
            catch (Exception ex)
            {
                return new EntityCollection();
            }
        }

        // GET api/<controller>/5
        /// <summary>
        /// method to Retrieve contact by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public EntityCollection RetrieveContactById(Guid id)
        {
            try
            {
                var contact = _contactService.RetrieveContactById(id);
                return contact;
            }
            catch (Exception ex)
            {
                return new EntityCollection();
            }
        }

        // GET api/<controller>/5
        /// <summary>
        /// method to Retrieve contact by first name
        /// </summary>
        /// <param name="firstName"></param>
        /// <returns></returns>
        [HttpGet]
        public EntityCollection RetrieveByFirstName(string firstName)
        {
            try
            {
                var contacts = _contactService.RetrieveByFirstName(firstName);
                return contacts;
            }
            catch (Exception ex)
            {
                return new EntityCollection();
            }
        }

        // GET api/<controller>/5
        /// <summary>
        /// method to Retrieve contact by last name
        /// </summary>
        /// <param name="lastName"></param>
        /// <returns></returns>
        [HttpGet]
        public EntityCollection RetrieveByLastName(string lastName)
        {
            try
            {
                var contacts = _contactService.RetrieveByLastName(lastName);
                return contacts;
            }
            catch (Exception ex)
            {
                return new EntityCollection();
            }
        }

        // GET api/<controller>/5
        /// <summary>
        /// method to Retrieve contact by email Id
        /// </summary>
        /// <param name="emailId"></param>
        /// <returns></returns>
        [HttpGet]
        public EntityCollection RetrieveByEmailId(string emailId)
        {
            try
            {
                var contacts = _contactService.RetrieveByEmailId(emailId);
                return contacts;
            }
            catch (Exception ex)
            {
                return new EntityCollection();
            }
        }

        // GET api/<controller>/5
        /// <summary>
        /// method to Retrieve contact by Contact Number
        /// </summary>
        /// <param name="contactNumber"></param>
        /// <returns></returns>
        [HttpGet]
        public EntityCollection RetrieveByContactNumber(string contactNumber)
        {
            try
            {
                var contacts = _contactService.RetrieveByContactNumber(contactNumber);
                return contacts;
            }
            catch (Exception ex)
            {
                return new EntityCollection();
            }
        }

        #endregion

        #region Add

        // POST api/<controller>
        /// <summary>
        /// method to Add Contact
        /// </summary>
        /// <param name="contactModel"></param>
        /// <returns></returns>
        [HttpPost]
        public string AddContact([FromBody] List<ContactModel> contactModel)
        {
            try
            {
                foreach(var contact in contactModel)
                {
                    if (contactModel != null && !string.IsNullOrWhiteSpace(contact.FirstName))
                    {
                        var contactId = _contactService.AddContact(contact);
                        InterestService interestService = new InterestService();
                        contactId = interestService.AddInterest(contact, contactId);
                        _participationService.AddParticipation(contactId);
                        contactId = _eventService.AddEventDetails(contact, contactId);
                    }
                }
                return ("Contacts added");
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        #endregion

        #region Update

        // Activate/Deactivate Contact
        /// <summary>
        /// method to Activate or Deactivate Contact
        /// </summary>
        /// <param name="contactId"></param>
        [HttpPut]
        public void ActivateDeactivateContact(Guid contactId)
        {
            try
            {
                _contactService.ActivateDeactivateContact(contactId);
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region Delete

        // DELETE api/<controller>/5
        /// <summary>
        /// method to Delete Contact
        /// </summary>
        /// <param name="contactId"></param>
        [HttpDelete]
        public void DeleteContact(Guid contactId)
        {
            try
            {
                _contactService.DeleteContact(contactId);
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}