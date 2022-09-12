# BitButter C.O.R.E V2

小黄油框架轻量版，采用工厂模式与事件驱动。

#### 对象工厂

继承自BaseObject的游戏对象类可以被ObjectFactory工厂进行全生命周期管理，开发者不需要对其进行额外维护。

例如，我们定义游戏的角色类Character：
``` cs
public class Character : BaseObject
{
    // 确保构造函数为public
    public Character(uint id)
        : base(id)
    {
    }
}
```

我们便可以通过ObjectFactory实例化Character类的对象：
``` cs
var character = ObjectFactory.Instance.Create<Character>();
```

变量character是指向Character对象的ObjectReference，通过其Object属性可访问Character对象：
``` cs
var characterObject = character.Object;
```

注意，继承自BaseObject的类无法通过new关键字实例化：
``` cs
var characterObject = new Character(newID); // 将抛出InvalidOperationException
```

使用ObjectReference作为参数，可以删除工厂中的对象：
``` cs
var character = ObjectFactory.Instance.Create<Character>();
ObjectFactory.Instance.Remove(character); // 删除character指向的对象
```
删除对象后，ObjectReference.IsValid将返回False，且ObjectReference.Object将返回null。

#### 对象查询

我们定义以下游戏对象类：
``` cs
// 角色抽象基类
public abstract class Character : BaseObject
{
    public Character(uint id)
        : base(id)
    {
    }

    public abstract string Name { get; }
}

// 英雄类，继承自角色基类
public class Hero : Character
{
    public Hero(uint id)
        : base(id)
    {
    }

    public override string Name => "Hero";
}

// 怪物类，继承自角色基类
public class Monster : Character
{
    public Monster(uint id, int attack)
        : base(id)
    {
        Attack = attack;
    }

    public override string Name => "Monster";
    
    public int Attack { get; }
}
```

可以通过ObjectFactory的Query方法进行对象查询，示例如下：
``` cs
// 实例化游戏对象
ObjectFactory.Instance.Create<Hero>();
ObjectFactory.Instance.Create<Monster>(1);
ObjectFactory.Instance.Create<Monster>(3);

// 按类型查询
var allObjects = ObjectFactory.Instance.Query<BaseObject>(); // 返回工厂中所有对象实例的ObjectReference
var allCharacters = ObjectFactory.Instance.Query<Character>(); // 返回工厂中所有角色类实例的ObjectReference，在这个例子中查询结果与上一条查询相同
var allHeros = ObjectFactory.Instance.Query<Hero>(); // 返回工厂中所有英雄类实例的ObjectReference

// 条件查询
var validMonsters = ObjectFactory.Instance.Query<Monster>(monster => monster.Attack >= 3); // 返回工厂中所有攻击力大于等于3的怪物类实例的ObjectReference
```

#### 抛出事件

使用EventManager的RaiseEvent方法抛出事件，示例如下：
``` cs
EventManager.Instance.RaiseEvent("TestEventName");
```

所有事件均通过字符串进行唯一标识，可以使用字符串常量存储事件标识，确保不会因拼写错误导致事件处理出错。

抛出事件时可以提供参数，示例如下：
``` cs
EventManager.Instance.RaiseEvent("TestEventName", 1, "abc", new[] { 0, 1 });
```

参数数量及类型均没有限制。对于同一个事件，也可以在多次抛出时传入不同数量及类型的参数。

#### 注册事件Handler

使用EventManager的AddHandler方法注册事件Handler，示例如下：
``` cs
EventManager.Instance.AddHandler("TestEventName", (args) =>
{
    // Handle event
});
```

上例中使用lambda表达式定义了Handler方法，也可以使用带有可变参数的方法，示例如下：
``` cs
EventManager.Instance.AddHandler("TestEventName", HandleEvent);

void HandleEvent(params object[] args)
{
    // Handle event
}
```

在事件Handler中，可以使用args参数获取事件数据，但开发者必须确保访问args参数时不会造成下标越界。

如果事件Handler不会使用args参数，可以使用"_"忽略事件参数。
``` cs
EventManager.Instance.AddHandler("TestEventName", (_) =>
{
    // Handle event without parameters
});
```

另外，不仅在框架提供的BaseObject子类中可以注册事件Handler，在一般C#类中也可以注册事件Handler，即框架的事件模块可以单独使用。