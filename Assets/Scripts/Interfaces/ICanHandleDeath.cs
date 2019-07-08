using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripleBladeHorse
{
	public interface ICanHandleDeath
	{
		void OnDeath(CharacterState state);
	}
}
