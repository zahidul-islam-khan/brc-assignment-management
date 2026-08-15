export type Role = "admin" | "teacher" | "student"
export type Group = "Science" | "Business Studies" | "Humanities"

export type UserStatus = "Active" | "Inactive" | "Suspended"
export type AssignmentStatus = "Draft" | "Published" | "Closed"
export type SubmissionStatus =
  | "Not Submitted"
  | "Draft"
  | "Submitted"
  | "Late"
  | "Graded"
  | "Returned"

export const INSTITUTION = "Bengal Renaissance College"

export interface Person {
  id: string
  name: string
  email: string
  phone: string
  role: Role
  klass?: string
  group?: Group
  status: UserStatus
  created: string
  studentId?: string
}

export interface Klass {
  id: string
  name: string
  program: string
  group: Group
  year: string
  students: number
  teachers: number
  status: "Active" | "Inactive"
  description: string
}

export interface Subject {
  code: string
  name: string
  klass: string
  group: Group
  teacher: string
  credits: number
  status: "Active" | "Inactive"
}

export interface Assignment {
  id: string
  title: string
  subject: string
  klass: string
  teacher: string
  deadline: string
  marks: number
  status: AssignmentStatus
  submissions: number
  total: number
  description: string
}

export interface Submission {
  id: string
  student: string
  assignment: string
  subject: string
  klass: string
  teacher: string
  submittedAt: string
  status: SubmissionStatus
  marks: number | null
  feedback?: string
}

export const groups: { name: Group; subjects: string[]; classes: number; students: number }[] = [
  { name: "Science", subjects: ["Physics", "Chemistry", "Higher Mathematics", "Biology"], classes: 3, students: 128 },
  { name: "Business Studies", subjects: ["Accounting", "Finance, Banking and Insurance", "Business Organization and Management"], classes: 2, students: 84 },
  { name: "Humanities", subjects: ["Economics", "Civics and Good Governance", "History", "Geography", "Logic"], classes: 2, students: 62 },
]

export const currentUsers: Record<Role, Person> = {} as any;

export const users: Person[] = []

export const classes: Klass[] = []

export const subjects: Subject[] = []

export const assignments: Assignment[] = []

export const submissions: Submission[] = []

/** Assignments as seen from the demo student (Fahim Rahman — XI Science A). */
export const studentAssignments = []

export const notifications = []

export const recentActivity = []
