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

        public Guid AddEvent(EventModel eventModel)
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
                    if (!string.IsNullOrWhiteSpace(eventModel.Description))
                        eventEntity.Attributes["sl_description"] = eventModel.Description;

                    Guid eventVenueId = CreateEventVenue(eventModel);
                    if (new Guid() != eventVenueId && null != eventVenueId)
                        eventEntity["sl_eventvenueid"] = new EntityReference("sl_eventvenue", eventVenueId);

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

        public Guid AddEventDetails(ContactModel contactModel, Guid contactId)
        {
            try
            {
                Guid emptyGuid = new Guid();
                var eventId = contactModel.EventId;
                if (contactModel != null && !string.IsNullOrWhiteSpace(contactModel.FirstName) && 
                    null != eventId && emptyGuid != eventId)
                {
                    if (contactModel.Roles != null && contactModel.Roles.Count > 0)
                    {
                        // Add contact to Event Attendee
                        foreach (var role in contactModel.Roles)
                        {
                            if ("Attendee" == role)
                            {
                                Entity attendeeEntity = new Entity("sl_eventattendee");
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

        public Guid CreateEventVenue(EventModel eventModel)
        {
            if (null != eventModel.Venue)
            {
                var venuequery = new QueryExpression("sl_eventvenue");
                venuequery.ColumnSet = new ColumnSet(true);
                FilterExpression venueChildFilterAND = venuequery.Criteria.AddFilter(LogicalOperator.And);

                string venueName = string.Empty;
                string city = string.Empty;
                string country = string.Empty;
                string state = string.Empty;
                string address = string.Empty;
                string displayAddress = string.Empty;

                foreach (KeyValuePair<string, string> kvp in eventModel.Venue)
                {
                    if (kvp.Value != null && "name" == kvp.Key)
                    {
                        venueName = kvp.Value;
                    }
                    if (kvp.Value != null && "city" == kvp.Key)
                    {
                        city = kvp.Value;
                    }
                    if (kvp.Value != null && "country" == kvp.Key)
                    {
                        country = kvp.Value;
                    }
                    if (kvp.Value != null && "state" == kvp.Key)
                    {
                        state = kvp.Value;
                    }
                    if (kvp.Value != null && "address1" == kvp.Key)
                    {
                        address = kvp.Value;
                    }
                    if (kvp.Value != null && "displayAddress" == kvp.Key)
                    {
                        displayAddress = kvp.Value;
                    }
                }

                venueChildFilterAND.AddCondition(new ConditionExpression("sl_name", ConditionOperator.Equal, venueName));
                venueChildFilterAND.AddCondition(new ConditionExpression("sl_city", ConditionOperator.Equal, city));
                venueChildFilterAND.AddCondition(new ConditionExpression("sl_country", ConditionOperator.Equal, country));
                venueChildFilterAND.AddCondition(new ConditionExpression("sl_stateprovince", ConditionOperator.Equal, state));
                venueChildFilterAND.AddCondition(new ConditionExpression("sl_addressline1", ConditionOperator.Equal, address));
                var venueList = _service.RetrieveMultiple(venuequery);

                if (venueList != null && venueList.Entities.Count == 0)
                {
                    Entity venueEntity = new Entity("sl_eventvenue");
                    if (!string.IsNullOrWhiteSpace(venueName))
                        venueEntity.Attributes["sl_name"] = venueName;
                    if (!string.IsNullOrWhiteSpace(state))
                        venueEntity.Attributes["sl_stateprovince"] = state;
                    if (!string.IsNullOrWhiteSpace(country))
                        venueEntity.Attributes["sl_country"] = country;
                    if (!string.IsNullOrWhiteSpace(city))
                        venueEntity.Attributes["sl_city"] = city;
                    if (!string.IsNullOrWhiteSpace(address))
                        venueEntity.Attributes["sl_addressline1"] = address;
                    if (!string.IsNullOrWhiteSpace(eventModel.SupportEmail))
                        venueEntity.Attributes["sl_email"] = eventModel.SupportEmail;

                    if (!string.IsNullOrWhiteSpace(displayAddress))
                    {
                        string[] displayAddressList = displayAddress.Split(',');
                        string postalCode = displayAddressList[2].Split(' ')[2];
                        venueEntity.Attributes["sl_postalcode"] = postalCode;
                    }

                    // Todo
                    //var venueAccountId = CreateAccount(venueName);
                    //if (new Guid() != venueAccountId && null != venueAccountId)
                    //    //venueEntity.Attributes["sl_venuenameid"] = venueAccountId;
                    //venueEntity["sl_venuenameid"] = new EntityReference("account", venueAccountId);

                    var venueId = _service.Create(venueEntity);
                    return venueId;

                }
            }
            return new Guid();
        }
        // Todo
        //public Guid CreateAccount(string accountName)
        //{
        //    if (!string.IsNullOrWhiteSpace(accountName))
        //    {
        //        var interestquery = new QueryExpression("account");
        //        interestquery.ColumnSet = new ColumnSet(true);
        //        interestquery.Criteria.AddCondition(new ConditionExpression("name", ConditionOperator.Equal, accountName));
        //        var interestList = _service.RetrieveMultiple(interestquery);
        //        if (interestList != null && interestList.Entities.Count == 0)
        //        {
        //            Entity accountEntity = new Entity("account");
        //            if (!string.IsNullOrWhiteSpace(accountName))
        //                accountEntity.Attributes["name"] = accountName;
        //            var venueId = _service.Create(accountEntity);
        //            return venueId;
        //        }
        //        return new Guid();
        //    }
        //    Logger.Info("returned new Guid() since new contact is null or first name is empty - InterestService.AddInterest");
        //    return new Guid();
        //}
    }
}