(function (thisArg, ajax, obj) {
	var s = {};
	thisArg.split('.').forEach(i => {
		s = i in window ? window[i] : window[i] = {};
	});
	function makeClass(n, i) {
		var oi = n[i];
		var ps = (oi.Properties || []).map(e=> { return { Name: e.Name, Value: null }; });
		n[i] = function (data) {
			for (var i of ps) {
				i.Value = data[i.Name] || null;
			}
		};
		ps.forEach(p=>
			Object.defineProperty(n[i].prototype, p.Name, {
				get: () => p.Value,
				set: v=>p.Value = v,
				enumerable: true
			}));
		oi.Methods.forEach(m=> {
			var ps = m.Parameters;
			n[i].prototype[m.Name] = function (args) {
				return function (later) {
					var url = "";
					var md;
					if (args instanceof HTMLFormElement) {
						md = "post";
						args = new FormData(args);
					} else {
						url += `?${ajax}=${m.Key}`;
						[...arguments].forEach((a, i) => url += `&${ps[i].Name}=${a}`);
						args = null;
						md = "get";
					}
					var r = new XMLHttpRequest();
					r.open(md, url);
					r.onload = () => {

					};
					r.send(args);
				};
			};
		});
	}
	function findClass(obj) {
		for (var i in obj) {
			var oi = obj[i];
			if ("Methods" in oi || "Properties" in oi) {
				makeClass(obj, i);
			} else {
				findClass(oi);
			}
		}
	}
	findClass(obj);
	for (var i in obj) {
		s[i] = obj[i];
	}
})(
"window",
"Gods1920065566",
{
  "BLL": {
    "V0": {
      "Live": {
        "Methods": [
          {
            "Name": "WakeUp",
            "Key": 8864859
          }
        ]
      },
      "Work": {
        "Methods": [
          {
            "Name": "GetType",
            "Key": 33189039,
            "Parameters": [
              {
                "Name": "type",
                "Type": "System.Type"
              }
            ],
            "ReturnType": [
              {
                "Name": "MemberType",
                "Type": "System.Reflection.MemberTypes"
              },
              {
                "Name": "DeclaringType",
                "Type": "System.Type"
              },
              {
                "Name": "DeclaringMethod",
                "Type": "System.Reflection.MethodBase"
              },
              {
                "Name": "ReflectedType",
                "Type": "System.Type"
              },
              {
                "Name": "StructLayoutAttribute",
                "Type": "System.Runtime.InteropServices.StructLayoutAttribute"
              },
              {
                "Name": "GUID",
                "Type": "System.Guid"
              },
              {
                "Name": "DefaultBinder",
                "Type": "System.Reflection.Binder"
              },
              {
                "Name": "Module",
                "Type": "System.Reflection.Module"
              },
              {
                "Name": "Assembly",
                "Type": "System.Reflection.Assembly"
              },
              {
                "Name": "TypeHandle",
                "Type": "System.RuntimeTypeHandle"
              },
              {
                "Name": "FullName",
                "Type": "System.String"
              },
              {
                "Name": "Namespace",
                "Type": "System.String"
              },
              {
                "Name": "AssemblyQualifiedName",
                "Type": "System.String"
              },
              {
                "Name": "BaseType",
                "Type": "System.Type"
              },
              {
                "Name": "TypeInitializer",
                "Type": "System.Reflection.ConstructorInfo"
              },
              {
                "Name": "IsNested",
                "Type": "System.Boolean"
              },
              {
                "Name": "Attributes",
                "Type": "System.Reflection.TypeAttributes"
              },
              {
                "Name": "GenericParameterAttributes",
                "Type": "System.Reflection.GenericParameterAttributes"
              },
              {
                "Name": "IsVisible",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsNotPublic",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsPublic",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsNestedPublic",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsNestedPrivate",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsNestedFamily",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsNestedAssembly",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsNestedFamANDAssem",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsNestedFamORAssem",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsAutoLayout",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsLayoutSequential",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsExplicitLayout",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsClass",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsInterface",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsValueType",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsAbstract",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsSealed",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsEnum",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsSpecialName",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsImport",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsSerializable",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsAnsiClass",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsUnicodeClass",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsAutoClass",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsArray",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsGenericType",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsGenericTypeDefinition",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsConstructedGenericType",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsGenericParameter",
                "Type": "System.Boolean"
              },
              {
                "Name": "GenericParameterPosition",
                "Type": "System.Int32"
              },
              {
                "Name": "ContainsGenericParameters",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsByRef",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsPointer",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsPrimitive",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsCOMObject",
                "Type": "System.Boolean"
              },
              {
                "Name": "HasElementType",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsContextful",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsMarshalByRef",
                "Type": "System.Boolean"
              },
              {
                "Name": "GenericTypeArguments",
                "Type": "System.Type[]"
              },
              {
                "Name": "IsSecurityCritical",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsSecuritySafeCritical",
                "Type": "System.Boolean"
              },
              {
                "Name": "IsSecurityTransparent",
                "Type": "System.Boolean"
              },
              {
                "Name": "UnderlyingSystemType",
                "Type": "System.Type"
              }
            ]
          },
          {
            "Name": "GoWork",
            "Key": 30265903
          }
        ]
      }
    }
  }
}
);
