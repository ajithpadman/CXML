///////////////////////////////////////////////////////////
//  CElement.cs
//  Implementation of the Class CElement
//  Generated by Enterprise Architect
//  Created on:      15-Dez-2017 13:29:33
//  Original author: ajith.padman
///////////////////////////////////////////////////////////




namespace CCodeTypes.Types
{
    public class CElement {
        /// <summary>
        /// MD5 hash code of the Element
        /// </summary>
        public string ID;

        /// <summary>
        /// hash of the element irrespective of location
        /// </summary>
        public string ElementID;
		/// <summary>
		/// Column number where the C element is located
		/// </summary>
		public int Column;
		/// <summary>
		/// Kind of the C element
		/// Function
		/// Datatype
		/// Varaible
		/// </summary>
		public ElementKind ElementKind;
		/// <summary>
		/// File where the C element is located
		/// </summary>
		public string File;
		/// <summary>
		/// Line number where the C element is located
		/// </summary>
		public int Line;
		/// <summary>
		/// Name of the element
		/// </summary>
		public string Name;

		public CElement(){

		}

		~CElement(){

		}

	}//end CElement

}//end namespace Types