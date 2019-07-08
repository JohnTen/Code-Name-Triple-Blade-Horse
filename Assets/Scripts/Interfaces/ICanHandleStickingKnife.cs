using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripleBladeHorse.Combat;

namespace TripleBladeHorse
{
	public interface ICanHandleStickingKnife
	{
		void OnStickingKnife(ICanStickKnife canStick, ThrowingKnife knife);
	}
}
