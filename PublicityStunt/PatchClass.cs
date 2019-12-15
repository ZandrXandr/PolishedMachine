using Mono.Cecil;
using MonoMod.InlineRT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoMod {

    //This is used to inform Partiality this is a patch. It doesn't actually do anything.
    [MonoMod.MonoModPatch( "" )]
    public class PatchSomething {

    }
    static class MonoModRules {
        static MonoModRules() {
            MonoModRule.Modder.PostProcessors += PostProcessor;
        }

        public static void PostProcessor(MonoModder modder) {
            foreach( TypeDefinition type in modder.Module.Types ) {
                PostProcessType( modder, type );
            }
        }

        private static void PostProcessType(MonoModder modder, TypeDefinition type) {
            foreach( FieldDefinition fdef in type.Fields )
                PostProcessField( modder, fdef );

            foreach( MethodDefinition mDef in type.Methods )
                PostProcessMethod( modder, mDef );

            foreach( TypeDefinition nested in type.NestedTypes )
                PostProcessType( modder, nested );

            foreach( PropertyDefinition pDef in type.Properties )
                PostProcessProperty( modder, pDef );

            if( type.IsNested )
                type.IsNestedPublic = true;
            else
                type.IsPublic = true;
        }

#pragma warning disable IDE0060
        private static void PostProcessField(MonoModder modder, FieldDefinition field) {
            field.IsPublic = true;
        }

        private static void PostProcessMethod(MonoModder modder, MethodDefinition mDef) {
            mDef.IsPublic = true;
        }

        private static void PostProcessProperty(MonoModder modder, PropertyDefinition pDef) {
            if( pDef.GetMethod != null )
                pDef.GetMethod.IsPublic = true;
            if( pDef.SetMethod != null )
                pDef.SetMethod.IsPublic = true;
        }
    }
}