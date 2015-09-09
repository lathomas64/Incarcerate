using UnityEngine;
using System.Collections;
using System.Xml.Serialization;

namespace IncrementalGame
{
	[XmlRoot("Resources")]
	public class Resource {

		public string name;
		public float capacity;
		float value;
		float income;
	}
}