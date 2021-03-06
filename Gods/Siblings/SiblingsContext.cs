﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Gods.Siblings {
    public abstract partial class SiblingsContext<M> : ISiblingsContext<M> where M : Model<M> {
        internal List<M> all { get; } = new List<M>();

        public void Add(M model) {
            if (typeof(M).GenericTypeArguments[0] != typeof(M)) {
                throw new ArithmeticException($"About type: {typeof(M).FullName}, write codes like this: class XXX : Model<XXX>{{ .. }}, not: class XXX : Model<YYY> {{ .. }}.");
            }
            if (model.Context == this) {
                return;
            }
            model.Context = this;
            model.records = new Dictionary<int, bool>();

            var ids = all.Select(m => m.id);
            var goneIds = Enumerable.Range(0, ids.Max()).Except(ids);
            model.id = goneIds.Any() ? goneIds.First() : all.Count;
            foreach (var s in all) {
                model.records[s.id] = model.CanSee(s, When.Join);
                var addToo = s.CanSee(model, When.NewMember);
                s.records[model.id] = addToo;
                if (addToo) {
                    s.OnSiblingJoined(model);
                }
            }
            all.Add(model);
        }
    }
}