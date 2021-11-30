一个简单的用于mvvm的数据模型，可实现模型--视图的双向绑定

优点：无反射！某些情况下，比如一个静态的模型，性能可能会比较好（只是可能...）

缺点：用起来没反射的双向绑定模型方便，不那么在意反射的可以去找找其他的库=L=

##使用方式：

创建一个DataModel(支持c#创建或json创建)

例:

```
{
    "name":"nilan",
    "status":{
        "attack":10,
        "defence":5
    }
    "items":[
        {
            "item_name":"sword",
            "item_count":1
        },
        {
            "item_name":"potion",
            "item_count":3
        }
    ]
}
```

在C#中可以直接通过Json格式创建模型，并通过路径查找、修改数据

```
var dataModel = new DataModel.FromJson(json);

//可以通过路径查找值
var name = dataModel.GetValue<string>("name");

//也可以通过路径修改值
var attack = dataModel.SetValue<int>("status/attack");

//可以添加监听事件，当数据被修改的时候会自动调用
dataModel.AddDataListener("status/attack",()=>{
    var val = dataModel.GetValue<int>("status/attack");
    print(val);
});

//通过下标访问数组中的元素
dataModel.GetValue<int>("items/0/item_count");
//注：因为比较懒
```

## 已知且大概率不会填的坑：

对于动态列表支持很烂，模型采用了地址索引的形式对元素进行管理

而每次导致下标变化的情况，会导致列表静态索引失效，于是下次查找又要重新建立静态索引了

不过设计初衷本来就是应对一些简单的ui界面，所以不管啦:P



