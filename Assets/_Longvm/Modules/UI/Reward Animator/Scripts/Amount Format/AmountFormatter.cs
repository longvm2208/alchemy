using System;
using UnityEngine;

namespace Toolkit
{
	[Serializable]
	public class AmountFormatter
	{
		[SerializeField] protected string format = "{0}";

        public virtual string Format(int amount) => string.Format(format, amount);
    } 
}
