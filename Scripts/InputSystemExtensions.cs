using System.Collections.Generic;
using UnityEngine.InputSystem;
	public static class InputSystemExtensions
	{

		/// <summary>
		/// Modified version of script from:<a href="https://doc.photonengine.com/fusion/current/technical-samples/fusion-vr-shared"> Photon Fusion Technical Samples - VR Shared </a>
		/// </summary>
		/// <remarks>
		/// <para>
		///  Add default binding if none are already set (in reference or manually set in action), and add XR prefix for left and right bindings.
		/// </para>
		/// </remarks>
		public static void EnableWithDefaultXRBindings(this InputActionProperty property, List<string> bindings = null, List<string> rightBindings = null)
		{
			if (property.reference == null && property.action.bindings.Count == 0)
			{
				const string xrPrefix = "<XRController>";
				if (bindings == null) bindings = new List<string>();
				if (rightBindings != null)
					foreach (var binding in rightBindings)
						bindings.Add(xrPrefix + "{RightHand}" + "/" + binding.TrimStart('/'));

				foreach (var binding in bindings)
				{
					property.action.AddBinding(binding);
				}
			}

			if (property.action != null) property.action.Enable();
		}

		public static void EnableWithDefaultXRBindings(this InputActionProperty property, List<string> bindings)
		{
			property.EnableWithDefaultXRBindings(rightBindings: bindings);
		}
	}
