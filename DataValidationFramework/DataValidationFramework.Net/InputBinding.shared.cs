﻿using System;
using System.Linq.Expressions;
using System.Reflection;

namespace RiaServicesContrib.DataValidation
{
    /// <summary>
    /// Class that represent a binding for a validation rule dependency
    /// 
    /// For a validation rule dependency
    ///    A => A.B.c
    ///    
    /// And an object
    ///    var a = new A { B = new B() };
    /// 
    /// TargetOwnerObject equals the object a.B,
    /// ParameterObjectBinding represents the binding of parameter 'A' to object 'a'.
    /// 
    /// </summary>
    internal class ValidationRuleDependencyBinding
    {
        /// <summary>
        /// Gets the ValidationRuleDependency of this ValidationRule dependency binding.
        /// </summary>
        public ValidationRuleDependency ValidationRuleDependency { get; internal set; }
        private object _targetOwnerObject;
        /// <summary>
        /// Represents the owning object of the target property 'p' of a validation rule dependency 'A => A.some.path.p'.
        /// </summary>
        public object TargetOwnerObject
        {
            get
            {
                if(_targetOwnerObject == null)
                {
                    _targetOwnerObject = GetTargetOwnerObject();
                } 
                return _targetOwnerObject;
            }
        }
        /// <summary>
        /// Represents the binding for the parameter 'A' of a validation rule dependency 'A => A.some.path.p'.
        /// </summary>
        public ParameterObjectBinding ParameterObjectBinding { get; internal set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if(obj is ValidationRuleDependencyBinding == false)
            {
                return false;
            }
            var binding = (ValidationRuleDependencyBinding)obj;
            if(binding.ValidationRuleDependency != ValidationRuleDependency && binding.ValidationRuleDependency.Equals(ValidationRuleDependency) == false)
            {
                return false;
            }
            if(binding.TargetOwnerObject != TargetOwnerObject && binding.TargetOwnerObject.Equals(TargetOwnerObject) == false)
            {
                return false;
            }
            if(binding.ParameterObjectBinding != ParameterObjectBinding && binding.ParameterObjectBinding.Equals(ParameterObjectBinding) == false)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int hashCode = base.GetHashCode();
            if(ValidationRuleDependency != null) hashCode ^= ValidationRuleDependency.GetHashCode();
            if(TargetOwnerObject != null) hashCode ^= TargetOwnerObject.GetHashCode();
            if(ParameterObjectBinding != null) hashCode ^= ParameterObjectBinding.GetHashCode();
            return hashCode;
        }
        private Object GetTargetOwnerObject()
        {
            var obj = ValidationRuleDependency.Expression.Body.Aggregate(
                ParameterObjectBinding.ParameterObject, (s, e) => GetObject(s, e));
            return obj;
        }
        private object GetObject(object seed, Expression expr)
        {
            if(expr is MemberExpression)
            {
                var propInfo = ((MemberExpression)expr).Member as PropertyInfo;
                if(propInfo == ValidationRuleDependency.TargetProperty)
                    return seed;
                return propInfo.GetValue(seed, null);
            }
            return seed;
        }
    }
}
