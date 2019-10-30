using System;
using Xplorer.States;

namespace Xplorer.Models
{
    public abstract class Transaction
    {
        protected Context Context { get; }

        protected Transaction(Context context)
        {
            Context = context;
        }

        public virtual void Start()
        {
            Context.Model.Transaction = this;
        }

        public virtual void Complete()
        {
            Context.Model.Transaction = null;
        }

        public virtual void Render()
        {
            Context.Component.Render(Context.State);
        }
    }
}