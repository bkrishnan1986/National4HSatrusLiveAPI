using log4net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using National4HSatrusLive.Models;
using System;
using System.Collections.Generic;

namespace National4HSatrusLive.Services
{
    public class EventService
    {
        private OrganizationServiceProxy _service;

        /// <summary>
        /// The logger
        /// </summary>
        private ILog Logger = LogManager.GetLogger(typeof(ContactService));

        public EventService()
        {
            _service = OrganizationService.GetCrmService();
        }

        public Guid CreateEvent(EventModel eventModel)
        {
            try
            {
                if (eventModel != null && !string.IsNullOrWhiteSpace(eventModel.Name))
                {
                    Entity eventEntity = new Entity("sl_event");

                    var query = new QueryExpression("sl_event");
                    query.ColumnSet = new ColumnSet(true);
                    FilterExpression incidentChildFilterOR = query.Criteria.AddFilter(LogicalOperator.Or);
                    FilterExpression incidentChildFilterAND = query.Criteria.AddFilter(LogicalOperator.And);
                    incidentChildFilterAND.AddCondition(new ConditionExpression("sl_name", ConditionOperator.Equal, eventModel.Name));
                    incidentChildFilterAND.AddCondition(new ConditionExpression("sl_startdate", ConditionOperator.Equal, eventModel.StartDate.Date));
                    incidentChildFilterAND.AddCondition(new ConditionExpression("sl_enddate", ConditionOperator.Equal, eventModel.EndDate.Date));

                    var queryResult = _service.RetrieveMultiple(query);

                    if (queryResult != null && queryResult.Entities.Count > 0)
                    {
                        // Todo - what happen if the event is already exist
                        return new Guid();
                    }

                    if (!string.IsNullOrWhiteSpace(eventModel.Name))
                        eventEntity.Attributes["sl_name"] = eventModel.Name;

                    DateTime? startDate = eventModel.StartDate;
                    if(startDate.HasValue)
                    {
                        eventEntity.Attributes["sl_startdate"] = startDate;
                    }

                    DateTime? endDate = eventModel.EndDate;
                    if(endDate.HasValue)
                    {
                        eventEntity.Attributes["sl_enddate"] = endDate;
                    }

                    var eventId = _service.Create(eventEntity);
                    return eventId;
                }
                Logger.Info("returned new Guid() since new contact is null or first name is empty - ContactService.AddContact");
                return new Guid();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new Guid();
            }
        }

        public Guid AddEvent(ContactModel contactModel, Guid contactId)
        {
            try
            {
                if (contactModel != null && !string.IsNullOrWhiteSpace(contactModel.FirstName))
                {
                    if (contactModel.Roles != null && contactModel.Roles.Count > 0)
                    {
                        // Add contact to Event Attendee
                        foreach (var role in contactModel.Roles)
                        {
                            if ("Attendee" == role)
                            {
                                Entity attendeeEntity = new Entity("sl_eventattendee");
                                var eventId = contactModel.EventId;
                                if (!isAttendeeExists(contactId, eventId))
                                {
                                    attendeeEntity["sl_eventid"] = new EntityReference("sl_eventattendee", eventId);
                                    attendeeEntity["sl_contactid"] = new EntityReference("sl_eventattendee", contactId);
                                    _service.Create(attendeeEntity);
                                }
                            }
                        }

                        // Add contact to Event Sponsor
                        foreach (var role in contactModel.Roles)
                        {
                            if ("Sponsor" == role)
                            {
                                Entity eventEntity = new Entity("sl_eventsponsor");
                                var eventId = contactModel.EventId;
                                if (!isSponserExists(contactId, eventId))
                                {
                                    eventEntity["sl_eventid"] = new EntityReference("sl_eventsponsor", eventId);
                                    eventEntity["sl_contactid"] = new EntityReference("sl_eventsponsor", contactId);
                                    _service.Create(eventEntity);
                                }
                            }
                        }

                        // Add contact to Event Speaker
                        foreach (var role in contactModel.Roles)
                        {
                            if ("Speaker" == role)
                            {
                                Entity eventEntity = new Entity("sl_eventspeaker");
                                var eventId = contactModel.EventId;
                                if (!isSpeakerExists(contactId, eventId))
                                {
                                    eventEntity["sl_eventid"] = new EntityReference("sl_eventspeaker", eventId);
                                    eventEntity["sl_contactid"] = new EntityReference("sl_eventspeaker", contactId);
                                    _service.Create(eventEntity);
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

        public bool isAttendeeExists(Guid contactId, Guid eventId)
        {
            try
            {
                var query = new QueryExpression("sl_eventattendee");
                query.ColumnSet = new ColumnSet(true);
                query.Criteria.AddCondition(new ConditionExpression("sl_contactid", ConditionOperator.Equal, contactId));
                query.Criteria.AddCondition(new ConditionExpression("sl_eventid", ConditionOperator.Equal, eventId));

                var eventAttendeeLists = _service.RetrieveMultiple(query);
                if (eventAttendeeLists != null && eventAttendeeLists.Entities.Count > 0)
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

        public bool isSponserExists(Guid contactId, Guid eventId)
        {
            try
            {
                var query = new QueryExpression("sl_eventsponsor");
                query.ColumnSet = new ColumnSet(true);
                query.Criteria.AddCondition(new ConditionExpression("sl_contactid", ConditionOperator.Equal, contactId));
                query.Criteria.AddCondition(new ConditionExpression("sl_eventid", ConditionOperator.Equal, eventId));

                var eventSponsorLists = _service.RetrieveMultiple(query);
                if (eventSponsorLists != null && eventSponsorLists.Entities.Count > 0)
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

        public bool isSpeakerExists(Guid contactId, Guid eventId)
        {
            try
            {
                var query = new QueryExpression("sl_eventspeaker");
                query.ColumnSet = new ColumnSet(true);
                query.Criteria.AddCondition(new ConditionExpression("sl_contactid", ConditionOperator.Equal, contactId));
                query.Criteria.AddCondition(new ConditionExpression("sl_eventid", ConditionOperator.Equal, eventId));

                var eventSpeakerLists = _service.RetrieveMultiple(query);
                if (eventSpeakerLists != null && eventSpeakerLists.Entities.Count > 0)
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