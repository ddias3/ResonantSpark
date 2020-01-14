using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace Service {
        public class AllServices {
            private Dictionary<Type, object> services;

            public AllServices() {
                this.services = new Dictionary<Type, object>();
            }

            public AllServices AddServiceAs<T>(object service) {
                this.services.Add(typeof(T), service);
                return this;
            }

            public T GetService<T>() {
                return (T) this.services[typeof(T)];
            }
        }
    }
}