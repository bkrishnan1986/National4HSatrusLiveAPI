using Microsoft.Crm.Sdk.Messages;
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
    public class EventController : ApiController
    {
        /// <summary>
        /// The contact service
        /// </summary>
        private readonly EventService _contactService;
        
        /// <summary>
        /// The event service
        /// </summary>
        private readonly EventService _eventService;

        public EventController()
        {
            _contactService = new EventService();
            _eventService = new EventService();
        }

        #region Add
        // POST api/<controller>
        /// <summary>
        /// method to Add Event
        /// </summary>
        /// <param name="eventModel"></param>
        /// <returns></returns>
        [HttpPost]
        public Guid AddEvent([FromBody] EventModel eventModel)
        {
            try
            {
                if (eventModel != null && !string.IsNullOrWhiteSpace(eventModel.Name))
                {
                    var eventId = _eventService.AddEvent(eventModel);
                    return eventId;
                }
                return new Guid();
            }
            catch (Exception ex)
            {
                return new Guid();
            }
        }
        #endregion

    }
}