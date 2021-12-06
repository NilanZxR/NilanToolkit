一个简单的脏标记系统

使用很简单
继承IDirtyMarkable接口

调用
DirtyMark.SetDirty(_**target**_)

LateUpdate中会自动调用对象的OnDirtyStateRefresh
并且将Dirty状态清除

也可以手动调用DirtyMark.Flush()，可以立即刷新所有脏标记元素

如果无法继承自接口，也可以通过DirtyMarkHandler使用