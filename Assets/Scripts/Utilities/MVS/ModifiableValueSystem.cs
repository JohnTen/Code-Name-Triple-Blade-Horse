using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JTUtility.MVS
{
	public enum ModificationType
	{
		Add,
		Mul_Add,
		Set,
	}

	[Serializable]
	public abstract class ModifiableValueSystem<E> where E : struct, IConvertible
	{
		[System.Serializable]
		protected class InnerValues : EnumBasedCollection<E, float> { }

		protected virtual InnerValues BasicValue { get; set; }
		protected virtual InnerValues ModifiedValue { get; set; }
		protected List<Modifier<E>> modifiers = new List<Modifier<E>>();

		public ModifiableValueSystem()
		{
			if (!typeof(E).IsEnum)
				throw new ArgumentException(typeof(E).Name + " is not a Enum type!");

			BasicValue = new InnerValues();
			ModifiedValue = new InnerValues();
		}

		public float GetBasicValue(E valueType)
		{
			return BasicValue[valueType];
		}

		public float SetBasicValue(E valueType, float value)
		{
			return BasicValue[valueType] = value;
		}
		
		public float GetModifiedValue(E valueType)
		{
			return ModifiedValue[valueType];
		}

		public float GetMultiplicationModify(E valueType)
		{
			float mul = 1;
			for (int i = 0; i < modifiers.Count; i ++)
			{
				if (!modifiers[i].propertyType.Equals(valueType) ||
					modifiers[i].modificationType != ModificationType.Mul_Add)
					continue;

				mul += modifiers[i].value_1;
			}

			return mul;
		}

		public float GetAdditionModify(E valueType)
		{
			float add = 0;
			for (int i = 0; i < modifiers.Count; i++)
			{
				if (!modifiers[i].propertyType.Equals(valueType) ||
					modifiers[i].modificationType != ModificationType.Add)
					continue;

				add += modifiers[i].value_1;
			}

			return add;
		}

		public virtual List<Modifier<E>> Modifiers
		{
			get { return modifiers; }
		}

		public virtual void MergeModifier(Modifier<E> modifier)
		{
			switch (modifier.modificationType)
			{
				case ModificationType.Add:
					BasicValue[modifier.propertyType] += modifier.value_1;
					break;
				case ModificationType.Mul_Add:
					BasicValue[modifier.propertyType] *= modifier.value_1;
					break;
				case ModificationType.Set:
					BasicValue[modifier.propertyType] = modifier.value_1;
					break;
			}
		}

		public virtual void AddModifier(Modifier<E> modifier)
		{
			modifiers.Add(modifier);
			CalculateModifiedValue();
		}
		
		public virtual void AddModifierWithoutRecalculate(Modifier<E> modifier)
		{
			modifiers.Add(modifier);
		}

		public virtual void RemoveModifier(Modifier<E> modifier)
		{
			modifiers.Remove(modifier);
			CalculateModifiedValue();
		}

		public virtual void RemoveModifierWithoutRecalculate(Modifier<E> modifier)
		{
			modifiers.Remove(modifier);
		}

		public virtual void CalculateModifiedValue()
		{
			InnerValues addition = new InnerValues();
			InnerValues multiplier = new InnerValues();
			InnerValues fixedValue = new InnerValues();

			var eValues = Enum.GetValues(typeof(E));
			for (int i = 0; i < eValues.Length; i ++)
			{
				addition[(E)eValues.GetValue(i)] = 0;
				multiplier[(E)eValues.GetValue(i)] = 1;
				fixedValue[(E)eValues.GetValue(i)] = float.NaN;
				ModifiedValue[(E)eValues.GetValue(i)] = BasicValue[(E)eValues.GetValue(i)];
			}

			for (int i = 0; i < modifiers.Count; i ++)
			{
				switch (modifiers[i].modificationType)
				{
					case ModificationType.Add:
						addition[modifiers[i].propertyType] += modifiers[i].value_1;
						break;
					case ModificationType.Mul_Add:
						multiplier[modifiers[i].propertyType] += modifiers[i].value_1;
						break;
					case ModificationType.Set:
						fixedValue[modifiers[i].propertyType] = modifiers[i].value_1;
						break;
				}
			}

			for (int i = 0; i < eValues.Length; i++)
			{
				if (!float.IsNaN(fixedValue[(E)eValues.GetValue(i)]))
				{
					ModifiedValue[(E)eValues.GetValue(i)] = fixedValue[(E)eValues.GetValue(i)];
					continue;
				}
				
				ModifiedValue[(E)eValues.GetValue(i)] = BasicValue[(E)eValues.GetValue(i)] * multiplier[(E)eValues.GetValue(i)];
				ModifiedValue[(E)eValues.GetValue(i)] += addition[(E)eValues.GetValue(i)];
			}
		}
	}

	[System.Serializable]
	public class Modifier<E> where E : struct, IConvertible
	{
		public E propertyType;
		public ModificationType modificationType;
		public float value_1;
		public float value_2;
		public float value_3;

		public Modifier() { }
		public Modifier(Modifier<E> modifier)
		{
			propertyType		= modifier.propertyType;
			modificationType	= modifier.modificationType;
			value_1				= modifier.value_1;
			value_2				= modifier.value_2;
			value_3				= modifier.value_3;
		}
	}
}
