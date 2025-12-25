using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace InterPlayers.Domain.Test.Helpers;

internal class SetIDHelper
{
    public static void SetId<T>(T entity, int id)
    {
        var prop = typeof(T).GetProperty(
            "Id",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );

        prop?.SetValue(entity, id);
    }
}
