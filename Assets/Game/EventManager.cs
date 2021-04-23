using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FranoW
{   
    namespace DevelopTools
    {
        public class EventManager
        {
            public delegate void EventReceiverParam(params object[] parameterContainer);

            private Dictionary<GameEvent, EventReceiverParam> eventsParam = new Dictionary<GameEvent, EventReceiverParam>();

            private Dictionary<GameEvent, Action> events = new Dictionary<GameEvent, Action>();


            public void SubscribeToEvent(GameEvent eventType, EventReceiverParam listener)
            {
                if (eventsParam == null)
                    eventsParam = new Dictionary<GameEvent, EventReceiverParam>();

                if (!eventsParam.ContainsKey(eventType))
                    eventsParam.Add(eventType, null);

                eventsParam[eventType] += listener;
            }

            public void SubscribeToEvent(GameEvent eventType, Action listener)
            {
                if (events == null)
                    events = new Dictionary<GameEvent, Action>();

                if (!events.ContainsKey(eventType))
                    events.Add(eventType, null);

                events[eventType] += listener;
            }

            public void UnsubscribeToEvent(GameEvent eventType, EventReceiverParam listener)
            {
                if (eventsParam != null)
                {
                    if (eventsParam.ContainsKey(eventType))
                        eventsParam[eventType] -= listener;
                }
            }

            public void UnsubscribeToEvent(GameEvent eventType, Action listener)
            {
                if (events != null)
                {
                    if (events.ContainsKey(eventType))
                        events[eventType] -= listener;
                }
            }

            public void TriggerEvent(GameEvent eventType)
            {
                TriggerEvent(eventType, null);
            }

            public void TriggerEvent(GameEvent eventType, params object[] parametersWrapper)
            {
                if (eventsParam.Count == 0 && events.Count == 0)
                {
                    return;
                }

                if (eventsParam.ContainsKey(eventType))
                {
                    if (eventsParam[eventType] != null)
                        eventsParam[eventType](parametersWrapper);
                }

                if (events.ContainsKey(eventType))
                {
                    if (events[eventType] != null)
                        events[eventType]();
                }
            }
        }
    }


}
