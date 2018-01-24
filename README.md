# Invio.Extensions.Reflection

[![Appveyor](https://ci.appveyor.com/api/projects/status/dst40kq4gbqx8k0e/branch/master?svg=true)](https://ci.appveyor.com/project/invio/invio-extensions-reflection/branch/master)
[![Travis CI](https://img.shields.io/travis/invio/Invio.Extensions.Reflection.svg?maxAge=3600&label=travis)](https://travis-ci.org/invio/Invio.Extensions.Reflection)
[![NuGet](https://img.shields.io/nuget/v/Invio.Extensions.Reflection.svg)](https://www.nuget.org/packages/Invio.Extensions.Reflection/)
[![Coverage](https://coveralls.io/repos/github/invio/Invio.Extensions.Reflection/badge.svg?branch=master)](https://coveralls.io/github/invio/Invio.Extensions.Reflection?branch=master)

Inspired by a [blog post by Jon Skeet](https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/), this .NET Core library includes a collection of extensions methods to the `FieldInfo`, `PropertyInfo`, `ConstructorInfo`, and `MethodInfo` types in the `System.Reflection` namespace.  These methods make repeated manipulation, creation, and utilization of data through these `MemberInfo`-types faster by "caching" their use via Func or Action delegates.

The `ReflectionHelper` classes provide methods for retrieving members via reflection using method groups and expression trees. This maximizes specificity when identifying members and increases compile time checks. For more information see the [Example Usage](reflectionhelper) section below

# Installation
The latest version of this package is available on NuGet. To install, run the following command:

```shell
PM> Install-Package Invio.Extensions.Reflection
```
# Example Usage

## MemberInfo Extension Methods

"[Reflection is slow.](http://www.manuelabadia.com/blog/PermaLink,guid,772c7152-b00e-4334-b677-bfbdcd8e6b5d.aspx)" As a C# developer you surely know this, but sometimes it is necessary to prevent a lot boilerplate code. For example, say you have a class where you want to allow callers to fetch the value for an arbitrary property, like so:

```csharp
public class MyImmutable {

    public String Name { get; private set; }
    public int Count { get; private set; }
    
    public MyImmutable(String name, int count) {
        this.Name = name;
        this.Count = count;
    }

    public object GetValue(String propertyName) {
        if (propertyName == "Name") {
            return this.Name;
        }
        
        if (propertyName == "Count") {
            return this.Count;
        }
        
        throw new ArgumentException($"The property '{propertyName}' does not exist.");
    }

}
```

If I have to add an additional property, I have to update the code in two places. In one place I have to add the new property to the class, and in the `GetValue()` method I have to add an additional branch to check for the property name. This is annoying, but certainly doable:

```csharp
public class MyImmutable {

    public String Name { get; private set; }
    public int Count { get; private set; }
    public decimal Cost { get; private set; }
    
    public MyImmutable(String name, int count, decimal cost) {
        this.Name = name;
        this.Count = count;
        this.Cost = cost;
    }

    public object GetValue(String propertyName) {
        if (propertyName == "Name") {
            return this.Name;
        }
        
        if (propertyName == "Count") {
            return this.Count;
        }
        
        if (propertyName == "Cost") {
            return this.Cost;
        }
        
        throw new ArgumentException($"The property '{propertyName}' does not exist.");
    }

}
```

Now, what if I wanted to add ten more properties, or had to do this process for ten more entities? What if I wanted to change my property lookup to be case sensitive? While this approach is faster in execution than using reflection, it restricts our ability to rapidly make changes to generic code that makes it decisions at runtime. Let's try this again with a reflection-based approach:

```csharp
public class MyImmutable {

    private static IDictionary<String, PropertyInfo> properties { get; }
    
    static MyImmutable() {
        properties = 
            typeof(MyImmutable)
                .GetProperties()
                .ToDictionary(property => property.Name);
    }

    public String Name { get; private set; }
    public int Count { get; private set; }
    public decimal Cost { get; private set; }
    
    public MyImmutable(String name, int count, decimal cost) {
        this.Name = name;
        this.Count = count;
        this.Cost = cost;
    }

    public object GetValue(String propertyName) {
        PropertyInfo property;
        
        if (!properties.TryGetValue(propertyName, out property)) {
            throw new ArgumentException($"The property '{propertyName}' does not exist.");
        }
        
        return property.GetValue(this);
    }

}
```

This lowers our line count and improves our ability to make consistent changes to how property values are found and retrieved in a generic way. Unfortunately, now that we are using reflection, we will pay a performance penalty for that privilege. This is where the `Invio.Extensions.Reflection` comes in. Here I can use the `CreateGetter<TBase>()` extension method in [`PropertyInfoExtensions`](src/Invio.Extensions.Reflection/PropertyInfoExtensions.cs) to cache access to this property into a delegate, which [has been shown to drop the performance penalty to as low as 10%](https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/) for the resultant execution time.

```csharp
using Invio.Extensions.Reflection;

public class MyImmutable {

    private static IDictionary<String, Func<MyImmutable, object>> getters { get; }
    
    static MyImmutable() {
        getters = 
            typeof(MyImmutable)
                .GetProperties()
                .ToDictionary(property => property.Name, property => property.CreateGetter<MyImmutable>());
    }

    public String Name { get; private set; }
    public int Count { get; private set; }
    public decimal Cost { get; private set; }
    
    public MyImmutable(String name, int count, decimal cost) {
        this.Name = name;
        this.Count = count;
        this.Cost = cost;
    }

    public object GetValue(String propertyName) {
        Func<MyImmutable, object> getter;
        
        if (!getters.TryGetValue(propertyName, out getter)) {
            throw new ArgumentException($"The property '{propertyName}' does not exist.");
        }
        
        return getter(this);
    }

}
```

Now we can get the benefits of generic code while discarding (most of) the performance penalties of reflection-based accessors.

There are similar extensions for [ConstructorInfo](src/Invio.Extensions.Reflection/ConstructorInfoExtensions.cs) objects ...

```csharp

var createMyClass =
     typeof(MyClass)
         .GetConstructors()
         .Single()
         .CreateFunc1<MyClass>();

var myClass = createMyClass(1);

public class MyClass {

    public int Foo { get; private set; }

    public MyClass(int foo) {
        this.Foo = foo;
    }
}

```

... as well as [MethodInfo](src/Invio.Extensions.Reflection/MethodInfoExtensions.cs) objects ...

```csharp

var myObject = new MyClass();

var setFoo =
     typeof(MyClass)
         .GetMethod("SetFoo")
         .CreateFunc1<MyClass>();

var nextObject = setFoo(myObject, 2);

public class MyClass {

    public int Foo { get; private set; }

    public MyClass(int foo) {
        this.Foo = foo;
    }
    
    public MyClass SetFoo(int foo) {
        return new MyClass(foo);
    }
    
}
```

... and even [FieldInfo](src/Invio.Extensions.Reflection/FieldInfoExtensions.cs) objects ...

```csharp

var myObject = new MyClass();

var setFoo =
     typeof(MyClass)
         .GetField("Foo")
         .CreateSetter();

setFoo(myObject, 2);

public class MyClass {

    public int Foo;
    
}
```

That's it. <3

## ReflectionHelper

There are times when a `MemberInfo` instance needs to be retrieved for a method or property that is known and accessible at compile time. One of the most common reasons for this is when a generic method needs to be invoked dynamically with a type that is only known at runtime. However, using the built in reflection methods on `Type` for this is problematic for a few reasons. For one thing, in the case of retrieving a method, it is important to be explicit about the arguments that a method takes or the addition of an overload could cause the call to `Type.GetMethod` to fail. The syntax for calling `GetMethod` with visibility and argument types specified is quite verbose. Additionally there are many changes that could be made to the method being retrieved, which could be detected as breaking changes at compile time, but are not detected until runtime when using the built in reflection methods.

### GetMethodFromExpression

Here is a simple example of a case where usage of `Type.GetMethod` should be replaced using `ReflectionHelper`.

```csharp
public class ListUtility {
    // Assume we had some generic function
    public static TList CloneListGeneric<TList, T>(TList items)
        where TList : ICollection<T>, new() {

        var list = new TList();
        foreach (var item in items) {
            list.Add(item);
        }

        return list;
    }
    
    // And we wanted to utilize that function from a context where we didn't know the type TList until runtime (not that we cannot invoke the other overload of CloneGenericList directly because interfaces types don't satisfy the new() constraint
    public ICollection<T> CloneList<T>(ICollection<T> list) {
    	 // We can use reflection to invoke the generic implementation dynamically
        return
            (ICollection<T>)typeof(ListUtil)
                .GetMethod(nameof(CloneListGeneric))
                .MakeGenericMethod(list.GetType())
                .Invoke(null, new[] { list });
    }
}
```

Unfortunately there are numerous ways that innocuous changes to the `CloneListGeneric` function could break our reflection based invocation:

 * An optional parameter such as `Predicate<T> filter = null` could be added
 * An overload that takes different arguments could be added
 * The argument type could be changed (in this case, not very practical, but perhaps `CloneListGeneric` might be modified to take `T[]` instead of `TList`)

If, instead, the function used ReflectionHelper, these problems would either not cause any problem, or be caught at compile time.

```csharp
using static Invio.Extensions.Reflection.ReflectionHelper;

public class ListUtil {
    // ...
    
    public static ICollection<T> CloneList<T>(ICollection<T> list) {
        return
            (ICollection<T>)
                GetMethodFromExpression(() =>
                        CloneListGeneric<List<Object>, Object>((List<Object>)null))
                    .GetGenericMethodDefinition()
                    .MakeGenericMethod(list.GetType(), typeof(T))
                    .Invoke(null, new[] { list });
    }
}
```

This particular example requires the types to be specified explicitly because of the dependent types, but in many cases where `ReflectionHelper` is useful the type arguments can be inferred.

```csharp
using static Invio.Extensions.Reflection.ReflectionHelper;

GetMethodFromExpression(() => Console.SetBuffer(0, 0));
GetMethodFromExpression(() => ImmutableList.Create(new int[0]));
```

### GetActionMethod/GetFuncMethod

While `GetMethodFromExpression` is extremely flexible and takes maximum advantage of type inference, it is not a particularly fast way to do reflection. For that reason the `GetActionMethod` and `GetFuncMethod` functions are provided to simplify getting a `MethodInfo` from a method group expression.

```csharp
// Without ReflectionHelper, method groups can be used to get a MethodInfo like so:
((Action<String>)Console.WriteLine).GetMethod()

// With ReflectionHelper this would look like
GetActionMethod<String>(Console.WriteLine)
```

Using a method group to retrieve a `MethodInfo` is up to 3 times faster than using an `Expression`. That said, heavy use of reflection can have a performance impact, and so you should consider caching `MethodInfo` objects and/or delegates constructed from `MethodInfo` objects when possible.
