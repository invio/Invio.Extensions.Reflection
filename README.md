# Invio.Extensions.Reflection

[![Appveyor](https://ci.appveyor.com/api/projects/status/dst40kq4gbqx8k0e/branch/master?svg=true)](https://ci.appveyor.com/project/invio/invio-extensions-reflection/branch/master)
[![Travis CI](https://img.shields.io/travis/invio/Invio.Extensions.Reflection.svg?maxAge=3600&label=travis)](https://travis-ci.org/invio/Invio.Extensions.Reflection)
[![NuGet](https://img.shields.io/nuget/v/Invio.Extensions.Reflection.svg)](https://www.nuget.org/packages/Invio.Extensions.Reflection/)
[![Coverage](https://coveralls.io/repos/github/invio/Invio.Extensions.Reflection/badge.svg?branch=master)](https://coveralls.io/github/invio/Invio.Extensions.Reflection?branch=master)

Inspired by a [blog post by Jon Skeet](https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/), this .NET Core library includes a collection of extensions methods to the `FieldInfo`, `PropertyInfo`, `ConstructorInfo`, and `MethodInfo` types in the `System.Reflection` namespace.  These methods make repeated manipulation, creation, and utilization of data through these `MemberInfo`-types faster by "caching" their use via Func or Action delegates.

# Installation
The latest version of this package is available on NuGet. To install, run the following command:

```shell
PM> Install-Package Invio.Extensions.Reflection
```
# Example Usage

"[Reflection is slow.](http://www.manuelabadia.com/blog/PermaLink,guid,772c7152-b00e-4334-b677-bfbdcd8e6b5d.aspx)" As a C# developer you surely know this, but sometimes it is necessary to prevent a lot boilerplate code. For example, say you have a class where you want to allow callers to fetch the value for an arbitrary property, like so:

```
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

```
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

Now, what if I wanted to add 10 more properties, or had to do this process for 10 more entities? What if I wanted to change my property lookup to be case sensitive? While all of thi boilerplate code is faster than reflection, but it restricts our ability to rapidly make changes to generic code that makes it decisions at runtime. Let's try this again with a reflection-based approach:

```
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

This lowers our line count and speeds up changes to how property values are found and retrieved in a generic way. Unfortunately, now we are using reflection, and we'll pay performance penalties for that privilege. This is where the `Invio.Extensions.Reflection` comes in. Here I can use the `CreateGetter<TBase>()` extension method in [`PropertyInfoExtensions`](src/Invio.Extensions.Reflection/PropertyInfoExtensions.cs) to cache access to this property into a delegate, which [has been shown to drop the performance penalty to as low as 10%](https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/) of execution time.

```
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

That's it. <3
