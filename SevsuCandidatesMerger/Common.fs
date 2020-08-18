namespace SevsuCandidatesMerger.Common

type Candidate = {
    Name: string;
    Score: int;
    IsPrivileged: bool;
}

type Course = {
    Name: string;
    Priority: int;
    Capacity: int;
}