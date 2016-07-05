#### cwsharp.freq
核心的词库，包含词+词频。可以修改词频的方式来修改最终词组的组合。

#### cwsharp.dic
扩展词典文件，每行一个词组。

#### cwsharp.dawg
DAWG词典。

#### 如何添加新的词组到字典
将新的词组添加到`cwsharp.dict`或自定义文本中，通过代码的方式重新生成DAWG文件。
```c#
 var rootPath = @"d:\"
var wordUtil = new WordUtil();
//加载默认的词频
using (var sr = new StreamReader(rootPath + @"\dict\cwsharp.freq", Encoding.UTF8))
{
	string line = null;
	while ((line = sr.ReadLine()) != null)
	{
		if (line == string.Empty) continue;
		var array = line.Split(' ');
		wordUtil.Add(array[0], int.Parse(array[1]));
	}            
}
//加载新的词典
using (var sr = new StreamReader(rootPath + @"\dict\cwsharp.dic", Encoding.UTF8))
{
	string line = null;
	while ((line = sr.ReadLine()) != null)
	{
		if (line == string.Empty) continue;
		wordUtil.Add(line);
	}
}
//保存新的dawg文件
wordUtil.SaveTo(file);
```