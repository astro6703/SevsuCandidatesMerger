module SevsuCandidatesMerger.Config

open SevsuCandidatesMerger.Common

let courses: Course list = [
        { Name = "09.03.02 Информационные системы и технологии"; Priority = 1; Capacity = 49; }
        { Name = "09.03.03 Прикладная информатика"; Priority = 2; Capacity = 22; }
        { Name = "09.03.01 Информатика и вычислительная техника"; Priority = 3; Capacity = 49; }
        { Name = "09.03.04 Программная инженерия"; Priority = 4; Capacity = 22; }
        { Name = "27.03.04 Управление в технических системах"; Priority = 5; Capacity = 40; }
        { Name = "15.03.04 Автоматизация технологических процессов и производств"; Priority = 6; Capacity = 14; }
    ]