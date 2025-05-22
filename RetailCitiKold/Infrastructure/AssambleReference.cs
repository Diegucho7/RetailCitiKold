using System.Reflection;

namespace RetailCitiKold.Infrastructure;

public static  class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}