# Задание
1. Построение многоугольника, полученного пересечением двух исходных. 
2. Построение многоугольника, полученного объединением двух исходных. 
3. Определение площади произвольного многоугольника.

# Билд
Билд приложения загружен на simmer.io
https://simmer.io/@Ilya_Leontev/topomatic-test-task

# Дополнения
1-2 Задания: 
Файл в проекте: PolygonClippingAlgorithm.cs

3 Задание: Использовалась формула площади Гаусса
https://ru.wikipedia.org/wiki/%D0%A4%D0%BE%D1%80%D0%BC%D1%83%D0%BB%D0%B0_%D0%BF%D0%BB%D0%BE%D1%89%D0%B0%D0%B4%D0%B8_%D0%93%D0%B0%D1%83%D1%81%D1%81%D0%B0
Файл в проекте: GeometryUtility.cs метод PolygonArea()

# P.S.
1. Алгоритм поддерживает Union и Intersection операции, но так же может считать и другие операции, такие как Difference, Xor, при дописании фильтра (пример в конце PolygonClippingAlgorithm.cs)
2. Обрабатывает случаи с совпадающими сегментами (идеальное перекрытие, ребро на ребре, общие вершины, один внутри другого и т.д.)
3. Касание вершины одного полгона о ребро или вершину другого полигона считается за пересечение
4. Ввод вершин может быть как по часовой, так и против часовой стрелки
5. При выборе опции Intersection, меньший полигон внутри большего полигона
 не является пересечением
6. Для визуализации извользован движок Unity


![JtwZzpRQ44](https://github.com/neerex/Topomatic-Test-Task/assets/48661254/408d837d-c20c-406f-8623-8d34b6d20094)
