///////////////////////////////////////////////////////////
//  IClang.cs
//  Implementation of the Interface IClang
//  Generated by Enterprise Architect
//  Created on:      15-Dez-2017 13:28:45
//  Original author: ajith.padman
///////////////////////////////////////////////////////////




using CCodeTypes.Types;

namespace CCodeFramework.Interfaces
{
    public interface IClang  {

		/// 
		/// <param name="File"></param>
		void Clang_FindFunctions(string File);

		/// 
		/// <param name="File"></param>
		void Clang_FindGlobalVariable(string File);

		/// 
		/// <param name="projectModel">Set the projectModel Reference</param>
		void Clang_SetProjectModel(ProjectModel projectModel);
	}//end IClang

}//end namespace Interfaces