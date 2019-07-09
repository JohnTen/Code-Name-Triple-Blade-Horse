using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TripleBladeHorse.Combat;

namespace TripleBladeHorse
{
	public interface ICanHandlePullingKnife
	{
		void OnPullingKnife(ICanStickKnife canStick, ThrowingKnife knife);
	}
}
