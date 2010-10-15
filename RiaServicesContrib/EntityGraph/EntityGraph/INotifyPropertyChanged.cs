﻿using System.ComponentModel;
using System.Reflection;
using System.ServiceModel.DomainServices.Client;
using System.Runtime.Serialization;

namespace RIA.EntityGraph
{
    public partial class EntityGraph<TEntity> : INotifyPropertyChanged
        where TEntity : Entity
    {
        /// <summary>
        /// handler to receive property changed events.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void SetupNotifyPropertyChangedHandlers()
        {
            foreach (var node in EntityRelationGraph)
            {
                node.PropertyChanged += node_PropertyChanged;
            }
        }
        private void RemoveNotifyPropertyChangedHandlers()
        {
            foreach (var node in EntityRelationGraph)
            {
                node.PropertyChanged -= node_PropertyChanged;
            }
        }
        void node_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyInfo propInfo;
            propInfo = sender.GetType().GetProperty(e.PropertyName);
            if (HasEntityGraphAttribute(propInfo))
            {
                EntityRelationGraphReset();
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(sender, e);
            }
        }
    }
}