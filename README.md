# SevsuCandidatesMerger

A tool to merge tables of applicants with the same subjects (only the ones that use the informatics,
math and russian for now) to get their distribution over the courses. Use this if you apply for
Sevastopol State University or know someone that does to see the worst-case scenario of your (his)
position. The applicants are distributed by next priorities:

1. 09.03.02 Информационные системы и технологии.
2. 09.03.03 Прикладная информатика.
3. 09.03.01 Информатика и вычислительная техника.
4. 09.03.04 Программная инженерия.
5. 27.03.04 Управление в технических системах.
6. 15.03.04 Автоматизация технологических процессов и производств.
7. 15.03.06 Мехатроника и робототехника.
8. 11.03.02 Инфокоммуникационные технологии и системы связи.
9. 11.03.04 Электроника и наноэлектроника.

# Usage

Use the next CLI to get the results from the Sevsu official applicants rating:

```bash
.\SevsuCandidatesMerger.exe --excel-file .\some-folder\11.08.20_бак_спец.xlsx
```
