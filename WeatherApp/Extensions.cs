using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp {

	public static class Extensions {

		/// <summary>
		/// Поиск диапазона индексов элементнов, для которых подходит данный ключ/запрос.
		/// Массив должен быть отсортирован и приведен к нижнему регистру
		/// </summary>
		public static Tuple<int, int> BinaryMinMaxIndexesSearch(this string[] list, string key) {
			if (list.Length == 0 || key == "") {
				return new Tuple<int, int>(-1, -1);
			}
			key = key.ToLower();
			int min = 0;
			int max = list.Length-1;
			int mid = 0;
			int matchIndex = -1;
			// задача цыкла - найти хотябы одно совпадения ключа с одним из элементов массива (совпадать должны все буквы вплоть до длинны ключа, если элемент массива короче ключа, то он сразу отбрасывается)
			while (min < max) {
				mid = min + ( ( max - min ) / 2 );
				if (list[mid].Length > key.Length){
					if (list[mid].Substring(0, key.Length) == key) {
						matchIndex = mid;
						break;
					}else{
						if (list[mid].CompareTo(key) > 0) {
							max = mid - 1;
						}
						else {
							min = mid + 1;
						}
					}
				}
				else {
					if (list[mid].CompareTo(key) > 0) {
						max = mid - 1;
					}
					else {
						min = mid + 1;
					}
				}
			}
			if (matchIndex >= 0) {
				///если вхождение найдено, двигаемся от него в обе стороны, пока не будет определен диапазон.
				min = matchIndex;
				max = matchIndex;
				while (min > 0) {
					if (list[min - 1].Length >= key.Length && list[min - 1].Substring(0, key.Length) == key) {
						min--;
					}
					else {
						break;
					}
				}
				while (max < list.Length - 1) {
					if (list[max + 1].Length >= key.Length && list[max + 1].Substring(0, key.Length) == key) {
						max++;
					}
					else {
						break;
					}
				}
				return new Tuple<int, int>(min, max);
			}
			return new Tuple<int, int>(-1, -1);
		}
	}
}
