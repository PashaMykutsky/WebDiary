using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organizer.Business
{
    public class PagedList<T>
    {
        private int maxItem_Page; // максимальное количество записей на странице
        private int pageCount; // сколько всего страниц
        public static int currentPage; // текущая страница
        private List<T> listObjects; // список всех записей пользователя 
        private T[] ObjectsToPage { get; set; } // массив записей на странице

        public PagedList(List<T> objects, int maxItemsPage)
        {
            ObjectsToPage = new T[maxItemsPage];
            listObjects = objects;
            maxItem_Page = maxItemsPage;
            Division_Pages();
        }

        public List<T> GetPageObjects()
        {
            if (currentPage > pageCount)
            {
                currentPage = pageCount;
            }
            else if (currentPage <= 0)
            {
                currentPage = 1;
            }
            if (currentPage <= pageCount)
            {
                int c = 0;
                int i = currentPage * maxItem_Page - maxItem_Page; // узнаем первый елемент вхождения
                int j = currentPage * maxItem_Page - 1; // узнаем последний елемент страницы

                for (int a = i; a <= j; a++)
                {
                    if (a + 1 == listObjects.Count)
                    {
                        ObjectsToPage[c] = listObjects[a];
                        break;
                    }
                    ObjectsToPage[c] = listObjects[a];
                    c++;
                }
                return DeleteNullValues(ObjectsToPage);
            }
            return null;
        }

        private List<T> DeleteNullValues(T[] recs)
        {
            List<T> r = new List<T>(); // массив для отбора данных / ячейки с null value не записываются
            for (int i = 0; i < recs.Length; i++)
            {
                if (recs[i] != null)
                {
                    r.Add(recs[i]);
                }
            }
            return r;
        }

        private void Division_Pages()
        {
            if (listObjects.Count > maxItem_Page)
            {
                if (listObjects.Count % maxItem_Page != 0)
                {
                    pageCount = listObjects.Count / maxItem_Page;
                    pageCount++;
                }
                else
                {
                    pageCount = listObjects.Count / maxItem_Page;
                }
            }
            else
            {
                pageCount = 1;
            }
        }
    }
}
