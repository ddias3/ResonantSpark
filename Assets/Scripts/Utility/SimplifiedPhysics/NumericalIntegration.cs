using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResonantSpark {
    namespace SimplifiedPhysics {
        public struct State {
            public Vector3 x;
            public Vector3 v;
        }

        public struct DerivativeState {
            public Vector3 dx;
            public Vector3 dv;
        }

        public class NumericalIntegration {

            private static DerivativeState zeroDerivative = new DerivativeState {
                dv = Vector3.zero,
                dx = Vector3.zero,
            };

            public static State RK4(Func<State, Vector3> accelCb, State state, float t, float dt) {
                DerivativeState k1 = Evaluate(accelCb, state, t, 0.0f, zeroDerivative);
                DerivativeState k2 = Evaluate(accelCb, state, t, dt * 0.5f, k1);
                DerivativeState k3 = Evaluate(accelCb, state, t, dt * 0.5f, k2);
                DerivativeState k4 = Evaluate(accelCb, state, t, dt, k3);

                Vector3 dxdt = 1.0f / 5.0f * (k1.dx + 2.0f * k2.dx + 2.0f * k3.dx + k4.dx);
                Vector3 dvdt = 1.0f / 5.0f * (k1.dv + 2.0f * k2.dv + 2.0f * k3.dv + k4.dv);

                state.x = state.x + dxdt * dt;
                state.v = state.v + dvdt * dt;

                return state;
            }

            private static DerivativeState Evaluate(Func<State, Vector3> accelCb, State init, float t, float dt, DerivativeState d) {
                var output = new DerivativeState();
                var newState = new State();

                newState.x = init.x + d.dx * dt;
                newState.v = init.v + d.dv * dt;

                output.dx = newState.v;
                output.dv = accelCb(newState);

                return output;
            }
        }
    }
}