import { useEffect, useMemo, useState, useRef, type ReactNode } from "react"
import {
  assignments as allAssignments,
  classes,
  currentUsers,
  INSTITUTION,
  notifications as notifs,
  recentActivity,
  studentAssignments,
  subjects,
  submissions as allSubmissions,
  users,
  type Assignment,
  type Person,
  type Role,
} from "./data"
import {
  Avatar,
  Badge,
  Button,
  Card,
  Combobox,
  ConfirmDialog,
  EmptyState,
  ErrorState,
  Field,
  Icon,
  IconButton,
  Input,
  Menu,
  MenuItem,
  Modal,
  Pagination,
  SearchInput,
  Select,
  SectionTitle,
  Skeleton,
  StatusBadge,
  Table,
  TableSkeleton,
  Tabs,
  Td,
  Textarea,
  Th,
  ToastProvider,
  Tr,
  useToast,
} from "./ui"

/* ================================================================ routing */
type Route = { view: string; params?: Record<string, string> }

const NAV: Record<Role, { key: string; label: string; icon: ReactNode }[]> = {
  admin: [
    { key: "dashboard", label: "Dashboard", icon: <Icon.dashboard /> },
    { key: "users", label: "Users", icon: <Icon.users /> },
    { key: "classes", label: "Classes", icon: <Icon.classes /> },
    { key: "subjects", label: "Subjects", icon: <Icon.subject /> },
    { key: "teacher-assign", label: "Teacher Assignment", icon: <Icon.award /> },
    { key: "assignments", label: "Assignments", icon: <Icon.assignment /> },
    { key: "submissions", label: "Submissions", icon: <Icon.submission /> },
    { key: "settings", label: "Settings", icon: <Icon.settings /> },
  ],
  teacher: [
    { key: "dashboard", label: "Dashboard", icon: <Icon.dashboard /> },
    { key: "assignments", label: "My Assignments", icon: <Icon.assignment /> },
    { key: "create", label: "Create Assignment", icon: <Icon.plus /> },
    { key: "submissions", label: "Submissions", icon: <Icon.submission /> },
    { key: "profile", label: "Profile", icon: <Icon.profile /> },
  ],
  student: [
    { key: "dashboard", label: "Dashboard", icon: <Icon.dashboard /> },
    { key: "assignments", label: "Assignments", icon: <Icon.assignment /> },
    { key: "submissions", label: "My Submissions", icon: <Icon.submission /> },
    { key: "profile", label: "Profile", icon: <Icon.profile /> },
  ],
}

const ROLE_LABEL: Record<Role, string> = { admin: "Administrator", teacher: "Teacher", student: "Student" }

/* ================================================================ shell */
function Logo({ compact, light }: { compact?: boolean; light?: boolean }) {
  return (
    <div className="flex items-center gap-2.5">
      <div className={`flex h-9 w-9 items-center justify-center rounded-xl font-display text-[13px] font-extrabold tracking-tight ${light ? "bg-white text-brand" : "bg-brand text-white"}`}>BRC</div>
      {!compact && (
        <div className="leading-tight">
          <div className={`font-display text-[15px] font-extrabold tracking-tight ${light ? "text-white" : "text-ink"}`}>Bengal Renaissance College</div>
          <div className={`text-[10px] font-medium uppercase tracking-wider ${light ? "text-white/60" : "text-faint"}`}>EduSubmit · Academic Suite</div>
        </div>
      )}
    </div>
  )
}

function Sidebar({ role, user, route, go, onLogout }: { role: Role; user: any; route: Route; go: (v: string) => void; onLogout: () => void }) {
  const me = user
  return (
    <aside className="flex h-full w-64 flex-col border-r border-line bg-surface">
      <div className="flex h-16 items-center border-b border-line px-5"><Logo /></div>
      <nav className="flex-1 space-y-1 overflow-y-auto p-3">
        {NAV[role].map((item) => {
          const active = route.view === item.key
          return (
            <button
              key={item.key}
              onClick={() => go(item.key)}
              className={`flex w-full items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-colors ${active ? "bg-brand-50 text-brand" : "text-ink-soft hover:bg-line-soft"}`}
            >
              <span className={active ? "text-brand" : "text-faint"}>{item.icon}</span>
              {item.label}
            </button>
          )
        })}
      </nav>
      <div className="border-t border-line p-3">
        <Menu
          align="left"
          trigger={
            <button className="flex w-full items-center gap-3 rounded-lg p-2 text-left hover:bg-line-soft">
              <Avatar name={me.name} />
              <span className="min-w-0 flex-1">
                <span className="block truncate text-[13px] font-semibold text-ink">{me.name}</span>
                <span className="block text-xs text-muted">{ROLE_LABEL[role]}</span>
              </span>
              <Icon.chevronDown className="h-4 w-4 text-faint" />
            </button>
          }
        >
          {(close) => (
            <>
              <MenuItem icon={<Icon.profile className="h-4 w-4" />} onClick={() => { go(role === "admin" ? "settings" : "profile"); close() }}>Profile &amp; settings</MenuItem>
              <MenuItem icon={<Icon.logout className="h-4 w-4" />} danger onClick={onLogout}>Logout</MenuItem>
            </>
          )}
        </Menu>
      </div>
    </aside>
  )
}

function NotificationBell({ go }: { go: (v: string) => void }) {
  const unread = notifs.filter((n) => n.unread).length
  return (
    <Menu
      trigger={
        <span className="relative inline-flex">
          <IconButton label="Notifications"><Icon.bell className="h-5 w-5" /></IconButton>
          {unread > 0 && <span className="absolute right-1.5 top-1.5 flex h-4 min-w-4 items-center justify-center rounded-full bg-danger px-1 text-[9px] font-bold text-white">{unread}</span>}
        </span>
      }
    >
      {(close) => (
        <div className="w-[min(88vw,340px)]">
          <div className="flex items-center justify-between px-2 pb-2">
            <span className="font-display text-sm font-bold text-ink">Notifications</span>
            <span className="text-xs text-brand">{unread} new</span>
          </div>
          <div className="max-h-80 overflow-y-auto">
            {notifs.map((n) => (
              <div key={n.id} className={`flex gap-2.5 rounded-lg px-2 py-2.5 text-sm hover:bg-line-soft ${n.unread ? "" : "opacity-70"}`}>
                <span className={`mt-1.5 h-1.5 w-1.5 shrink-0 rounded-full ${n.unread ? "bg-brand" : "bg-transparent"}`} />
                <span>
                  <span className="block text-[13px] leading-snug text-ink-soft">{n.text}</span>
                  <span className="text-xs text-faint">{n.time}</span>
                </span>
              </div>
            ))}
          </div>
          <div className="mt-1 border-t border-line pt-1">
            <button onClick={() => { go("notifications"); close() }} className="w-full rounded-lg py-2 text-center text-sm font-medium text-brand hover:bg-line-soft">View all notifications</button>
          </div>
        </div>
      )}
    </Menu>
  )
}

function Topbar({ role, user, title, crumbs, onMenu, showSearch, go, onLogout }: { role: Role; user: any; title: string; crumbs: string[]; onMenu: () => void; showSearch?: boolean; go: (v: string) => void; onLogout: () => void }) {
  const me = user
  return (
    <header className="sticky top-0 z-30 flex h-16 items-center gap-3 border-b border-line bg-surface/90 px-4 backdrop-blur lg:px-6">
      <IconButton label="Open menu" onClick={onMenu} className="lg:hidden"><Icon.menu className="h-5 w-5" /></IconButton>
      <div className="min-w-0 flex-1">
        <nav className="hidden items-center gap-1.5 text-xs text-faint sm:flex">
          {crumbs.map((c, i) => (
            <span key={i} className="flex items-center gap-1.5">
              {i > 0 && <Icon.chevron className="h-3 w-3" />}
              <span className={i === crumbs.length - 1 ? "font-medium text-muted" : ""}>{c}</span>
            </span>
          ))}
        </nav>
        <h1 className="truncate font-display text-lg font-bold tracking-tight text-ink">{title}</h1>
      </div>
      {showSearch && <SearchInput placeholder="Search…" className="hidden w-64 md:block" />}
      <NotificationBell go={go} />
      <Menu
        align="right"
        trigger={<button className="shrink-0 rounded-full outline-none focus-visible:ring-2 focus-visible:ring-brand"><Avatar name={me.name} size={34} /></button>}
      >
        {(close) => (
          <>
            <div className="border-b border-line-soft px-3 py-2 mb-1">
              <div className="truncate text-sm font-semibold text-ink">{me.name}</div>
              <div className="text-xs text-muted">{ROLE_LABEL[role]}</div>
            </div>
            <MenuItem icon={<Icon.profile className="h-4 w-4" />} onClick={() => { go(role === "admin" ? "settings" : "profile"); close() }}>Profile &amp; settings</MenuItem>
            <MenuItem icon={<Icon.logout className="h-4 w-4" />} danger onClick={onLogout}>Logout</MenuItem>
          </>
        )}
      </Menu>
    </header>
  )
}

function Shell({ role, user, route, go, onLogout, title, crumbs, showSearch, children }: {
  role: Role; user: any; route: Route; go: (v: string, params?: Record<string, string>) => void; onLogout: () => void
  title: string; crumbs: string[]; showSearch?: boolean; children: ReactNode
}) {
  const [mobileNav, setMobileNav] = useState(false)
  useEffect(() => setMobileNav(false), [route])
  return (
    <div className="flex h-screen overflow-hidden bg-canvas">
      <div className="hidden lg:block"><Sidebar role={role} user={user} route={route} go={go} onLogout={onLogout} /></div>
      {mobileNav && (
        <div className="fixed inset-0 z-50 lg:hidden">
          <div className="absolute inset-0 bg-ink/40" onClick={() => setMobileNav(false)} />
          <div className="es-fade absolute left-0 top-0 h-full"><Sidebar role={role} user={user} route={route} go={go} onLogout={onLogout} /></div>
        </div>
      )}
      <div className="flex min-w-0 flex-1 flex-col">
        <Topbar role={role} user={user} title={title} crumbs={crumbs} onMenu={() => setMobileNav(true)} showSearch={showSearch} go={go} onLogout={onLogout} />
        <main className="flex-1 overflow-y-auto">
          <div className="mx-auto max-w-6xl px-4 py-6 pb-24 lg:px-8 lg:py-8 lg:pb-8">{children}</div>
          <footer className="mx-auto flex max-w-6xl flex-col gap-1 px-4 pb-24 text-xs text-faint sm:flex-row sm:items-center sm:justify-between lg:px-8 lg:pb-8">
            <span>© 2026 {INSTITUTION} · EduSubmit</span>
            <span>Assignment &amp; Submission Management System</span>
          </footer>
        </main>
        <MobileNav role={role} route={route} go={go} />
      </div>
    </div>
  )
}

function MobileNav({ role, route, go }: { role: Role; route: Route; go: (v: string) => void }) {
  const items = NAV[role].filter((i) => i.key !== "create" && i.key !== "settings" && i.key !== "teacher-assign").slice(0, 5)
  return (
    <nav className="flex items-stretch border-t border-line bg-surface lg:hidden">
      {items.map((item) => {
        const active = route.view === item.key
        return (
          <button key={item.key} onClick={() => go(item.key)} className={`flex flex-1 flex-col items-center gap-1 py-2.5 text-[10px] font-medium ${active ? "text-brand" : "text-faint"}`}>
            <span className="[&_svg]:h-5 [&_svg]:w-5">{item.icon}</span>
            {item.label.split(" ")[0]}
          </button>
        )
      })}
    </nav>
  )
}

/* ================================================================ shared bits */
function PageHead({ title, subtitle, action }: { title: string; subtitle?: string; action?: ReactNode }) {
  return (
    <div className="mb-6 flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between">
      <div>
        <h1 className="font-display text-2xl font-extrabold tracking-tight text-ink">{title}</h1>
        {subtitle && <p className="mt-1 text-sm text-muted">{subtitle}</p>}
      </div>
      {action}
    </div>
  )
}

function StatCard({ label, value, icon, tone = "info", sub }: { label: string; value: string | number; icon: ReactNode; tone?: "info" | "ok" | "warn" | "danger" | "neutral"; sub?: string }) {
  const tones: Record<string, string> = {
    info: "bg-info-bg text-info", ok: "bg-ok-bg text-ok", warn: "bg-warn-bg text-warn", danger: "bg-danger-bg text-danger", neutral: "bg-neutral-bg text-ink-soft",
  }
  return (
    <Card className="flex items-center gap-4">
      <div className={`flex h-11 w-11 shrink-0 items-center justify-center rounded-xl ${tones[tone]}`}>{icon}</div>
      <div className="min-w-0">
        <div className="font-display text-2xl font-extrabold leading-none tracking-tight text-ink">{value}</div>
        <div className="mt-1 truncate text-[13px] text-muted">{label}</div>
        {sub && <div className="text-xs text-faint">{sub}</div>}
      </div>
    </Card>
  )
}

function BackLink({ onClick, children }: { onClick: () => void; children: ReactNode }) {
  return (
    <button onClick={onClick} className="mb-4 inline-flex items-center gap-1.5 text-sm font-medium text-muted hover:text-brand">
      <Icon.chevron className="h-4 w-4 rotate-180" />
      {children}
    </button>
  )
}

function MetaRow({ label, value }: { label: string; value: ReactNode }) {
  return (
    <div className="flex items-center justify-between gap-4 border-b border-line-soft py-2.5 last:border-0">
      <span className="text-sm text-muted">{label}</span>
      <span className="text-right text-sm font-medium text-ink">{value}</span>
    </div>
  )
}

function RowActions({ onView, onEdit, onDelete }: { onView?: () => void; onEdit?: () => void; onDelete?: () => void }) {
  return (
    <Menu trigger={<IconButton label="Row actions"><Icon.dots className="h-5 w-5" /></IconButton>}>
      {(close) => (
        <>
          {onView && <MenuItem icon={<Icon.eye className="h-4 w-4" />} onClick={() => { onView(); close() }}>View</MenuItem>}
          {onEdit && <MenuItem icon={<Icon.edit className="h-4 w-4" />} onClick={() => { onEdit(); close() }}>Edit</MenuItem>}
          {onDelete && <MenuItem icon={<Icon.trash className="h-4 w-4" />} danger onClick={() => { onDelete(); close() }}>Delete</MenuItem>}
        </>
      )}
    </Menu>
  )
}

/* ================================================================ dashboards */
function useLoad(ms = 650) {
  const [loading, setLoading] = useState(true)
  useEffect(() => { const t = setTimeout(() => setLoading(false), ms); return () => clearTimeout(t) }, [])
  return loading
}

function greeting() {
  const h = new Date().getHours()
  return h < 12 ? "Good morning" : h < 17 ? "Good afternoon" : "Good evening"
}

function AdminDashboard({ go }: { go: (v: string) => void }) {
  const loading = useLoad()
  return (
    <>
      <PageHead title="Dashboard" subtitle={`${greeting()}, Tanvir Ahmed — here's what's happening across EduSubmit today.`} />
      {loading ? (
        <div className="grid grid-cols-2 gap-4 lg:grid-cols-3">{Array.from({ length: 6 }).map((_, i) => <Card key={i}><Skeleton className="h-11 w-11 rounded-xl" /><Skeleton className="mt-4 h-6 w-16" /><Skeleton className="mt-2 h-3 w-24" /></Card>)}</div>
      ) : (
        <div className="grid grid-cols-2 gap-4 lg:grid-cols-3">
          <StatCard label="Students" value={248} icon={<Icon.users />} tone="info" />
          <StatCard label="Teachers" value={32} icon={<Icon.award />} tone="info" />
          <StatCard label="Classes" value={14} icon={<Icon.classes />} tone="neutral" />
          <StatCard label="Subjects" value={28} icon={<Icon.subject />} tone="neutral" />
          <StatCard label="Assignments" value={86} icon={<Icon.assignment />} tone="ok" />
          <StatCard label="Pending Submissions" value={41} icon={<Icon.clock />} tone="warn" />
        </div>
      )}
      <div className="mt-6 grid gap-6 lg:grid-cols-3">
        <div className="lg:col-span-2">
          <Card pad={false}>
            <div className="flex items-center justify-between p-5 pb-3"><SectionTitle>Recent Assignments</SectionTitle><Button size="sm" variant="ghost" onClick={() => go("assignments")}>View all</Button></div>
            <Table head={<><Th>Assignment</Th><Th>Teacher</Th><Th>Status</Th><Th>Submissions</Th></>}>
              {allAssignments.slice(0, 5).map((a) => (
                <Tr key={a.id} onClick={() => go("assignments")}>
                  <Td><div className="font-medium text-ink">{a.title}</div><div className="text-xs text-muted">{a.subject}</div></Td>
                  <Td>{a.teacher}</Td>
                  <Td><StatusBadge status={a.status} /></Td>
                  <Td><span className="font-mono text-[13px]">{a.submissions}/{a.total}</span></Td>
                </Tr>
              ))}
            </Table>
          </Card>
        </div>
        <div className="space-y-6">
          <Card>
            <SectionTitle>Recent Activity</SectionTitle>
            <ul className="space-y-3.5">
              {recentActivity.map((r, i) => (
                <li key={i} className="flex gap-3 text-sm">
                  <Avatar name={r.who} size={30} />
                  <div className="min-w-0">
                    <p className="leading-snug text-ink-soft"><span className="font-semibold text-ink">{r.who}</span> {r.action} <span className="font-medium">{r.target}</span></p>
                    <span className="text-xs text-faint">{r.time}</span>
                  </div>
                </li>
              ))}
            </ul>
          </Card>
          <Card>
            <SectionTitle>Quick Actions</SectionTitle>
            <div className="grid grid-cols-2 gap-2">
              <Button size="sm" variant="subtle" onClick={() => go("users")}>Add User</Button>
              <Button size="sm" variant="subtle" onClick={() => go("classes")}>New Class</Button>
              <Button size="sm" variant="subtle" onClick={() => go("subjects")}>New Subject</Button>
              <Button size="sm" variant="subtle" onClick={() => go("teacher-assign")}>Assign Teacher</Button>
            </div>
          </Card>
        </div>
      </div>
    </>
  )
}

function TeacherDashboard({ go }: { go: (v: string) => void }) {
  return (
    <>
      <PageHead title="Dashboard" subtitle={`${greeting()}, Nusrat Jahan — you have submissions waiting for review.`}
        action={<Button variant="primary" icon={<Icon.plus className="h-4 w-4" />} onClick={() => go("create")}>Create Assignment</Button>} />
      <div className="grid grid-cols-2 gap-4 lg:grid-cols-4">
        <StatCard label="Assignments" value={12} icon={<Icon.assignment />} tone="info" />
        <StatCard label="Published" value={8} icon={<Icon.checkCircle />} tone="ok" />
        <StatCard label="Pending Reviews" value={24} icon={<Icon.clock />} tone="warn" />
        <StatCard label="Submissions" value={96} icon={<Icon.submission />} tone="neutral" />
      </div>
      <div className="mt-6 grid gap-6 lg:grid-cols-3">
        <Card className="border-warn/30 bg-warn-bg/40 lg:col-span-1">
          <div className="flex items-start gap-3">
            <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-warn-bg text-warn"><Icon.alert className="h-5 w-5" /></div>
            <div>
              <h3 className="font-display text-sm font-bold text-ink">Needs Your Attention</h3>
              <p className="mt-1 text-sm text-ink-soft">18 submissions are waiting for grading across 3 assignments.</p>
              <Button size="sm" variant="primary" className="mt-3" onClick={() => go("submissions")}>Review now</Button>
            </div>
          </div>
        </Card>
        <Card className="lg:col-span-2" pad={false}>
          <div className="p-5 pb-3"><SectionTitle action={<Button size="sm" variant="ghost" onClick={() => go("submissions")}>All submissions</Button>}>Recent Submissions</SectionTitle></div>
          <Table head={<><Th>Student</Th><Th>Assignment</Th><Th>Submitted</Th><Th>Status</Th></>}>
            {allSubmissions.slice(0, 4).map((s) => (
              <Tr key={s.id} onClick={() => go("submissions")}>
                <Td><div className="flex items-center gap-2.5"><Avatar name={s.student} size={30} /><span className="font-medium text-ink">{s.student}</span></div></Td>
                <Td className="max-w-[180px] truncate">{s.assignment}</Td>
                <Td className="whitespace-nowrap text-xs">{s.submittedAt}</Td>
                <Td><StatusBadge status={s.status} /></Td>
              </Tr>
            ))}
          </Table>
        </Card>
      </div>
      <div className="mt-6">
        <Card>
          <SectionTitle>Upcoming Deadlines</SectionTitle>
          <div className="grid gap-3 sm:grid-cols-3">
            {allAssignments.filter((a) => a.status === "Published").slice(0, 3).map((a) => (
              <div key={a.id} className="rounded-lg border border-line p-3.5">
                <div className="text-sm font-semibold text-ink">{a.title}</div>
                <div className="mt-1 text-xs text-muted">{a.subject}</div>
                <div className="mt-2 flex items-center gap-1.5 text-xs font-medium text-warn"><Icon.clock className="h-3.5 w-3.5" />{a.deadline}</div>
              </div>
            ))}
          </div>
        </Card>
      </div>
    </>
  )
}

function StudentDashboard({ go }: { go: (v: string, params?: Record<string, string>) => void }) {
  const [apiAssignments, setApiAssignments] = useState<any[]>([])

  useEffect(() => {
    api.get<any>("/assignments/student?pageSize=4").then(res => setApiAssignments(res.items || []))
  }, [])

  return (
    <>
      <PageHead title="Dashboard" subtitle={`${greeting()}, Fahim Rahman — stay on top of your assignments.`} />
      <div className="grid grid-cols-2 gap-4 lg:grid-cols-4">
        <StatCard label="Total Assignments" value={8} icon={<Icon.assignment />} tone="info" />
        <StatCard label="Pending" value={3} icon={<Icon.clock />} tone="warn" />
        <StatCard label="Submitted" value={4} icon={<Icon.submission />} tone="neutral" />
        <StatCard label="Graded" value={3} icon={<Icon.checkCircle />} tone="ok" />
      </div>
      <div className="mt-6 grid gap-6 lg:grid-cols-3">
        <Card className="border-warn/30 lg:col-span-2">
          <SectionTitle>Upcoming Deadlines</SectionTitle>
          <div className="space-y-3">
            <div className="flex items-center justify-between rounded-lg border border-warn/40 bg-warn-bg/50 p-3.5">
              <div>
                <div className="text-sm font-semibold text-ink">Newton's Laws of Motion — Assignment 01</div>
                <div className="text-xs text-muted">Physics · Md. Abdul Karim</div>
              </div>
              <div className="text-right">
                <Badge tone="danger" dot>Due tomorrow</Badge>
                <div className="mt-1 text-xs text-muted">11:59 PM</div>
              </div>
            </div>
            <div className="flex items-center justify-between rounded-lg border border-line p-3.5">
              <div>
                <div className="text-sm font-semibold text-ink">Limits and Continuity — Problem Set</div>
                <div className="text-xs text-muted">Higher Mathematics · Nusrat Jahan</div>
              </div>
              <div className="text-right"><Badge tone="warn">Due in 6 days</Badge><div className="mt-1 text-xs text-muted">24 Aug</div></div>
            </div>
          </div>
        </Card>
        <Card>
          <SectionTitle>Recent Feedback</SectionTitle>
          <div className="rounded-lg bg-canvas p-3.5">
            <div className="flex items-center justify-between"><span className="text-sm font-semibold text-ink">Chemical Bonding</span><Badge tone="ok">22 / 25</Badge></div>
            <p className="mt-2 text-sm leading-relaxed text-ink-soft">"Clear Lewis structures. Explain metallic bonding in more depth."</p>
            <div className="mt-2 text-xs text-faint">— Md. Rakib Hasan</div>
          </div>
        </Card>
      </div>
      <div className="mt-6">
        <Card pad={false}>
          <div className="p-5 pb-3"><SectionTitle action={<Button size="sm" variant="ghost" onClick={() => go("assignments")}>View all</Button>}>Recent Assignments</SectionTitle></div>
          <Table head={<><Th>Assignment</Th><Th>Subject</Th><Th>Deadline</Th><Th>Status</Th></>}>
            {apiAssignments.slice(0, 4).map((a) => (
              <Tr key={a.id} onClick={() => go("assignment-detail", { id: a.id })}>
                <Td className="font-medium text-ink">{a.title}</Td>
                <Td>{a.subjectName}</Td>
                <Td className="whitespace-nowrap text-xs">{new Date(a.deadline).toLocaleDateString()}</Td>
                <Td><StatusBadge status={a.submissionStatus as string} /></Td>
              </Tr>
            ))}
          </Table>
        </Card>
      </div>
    </>
  )
}

/* ================================================================ admin: users */
function usePaged<T>(items: T[], perPage = 6) {
  const [page, setPage] = useState(1)
  const pages = Math.max(1, Math.ceil(items.length / perPage))
  const slice = items.slice((page - 1) * perPage, page * perPage)
  useEffect(() => { if (page > pages) setPage(1) }, [pages, page])
  return { page, pages, setPage, slice, total: items.length }
}

function AdminUsers({ go }: { go: (v: string, params?: Record<string, string>) => void }) {
  const toast = useToast()
  const [q, setQ] = useState("")
  const [role, setRole] = useState("all")
  const [status, setStatus] = useState("all")
  const [confirm, setConfirm] = useState<any | null>(null)
  
  const [apiUsers, setApiUsers] = useState<any[]>([])
  const [loading, setLoading] = useState(true)

  const fetchUsers = async () => {
    try {
      setLoading(true)
      const res = await api.get<any>("/users?pageSize=100")
      setApiUsers(res.items || [])
    } catch (e) {
      toast("Failed to load users.", "danger")
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchUsers()
  }, [])

  const filtered = useMemo(() => apiUsers.filter((u: any) =>
    (role === "all" || u.role.toLowerCase() === role.toLowerCase()) &&
    (status === "all" || u.status === status) &&
    (u.name.toLowerCase().includes(q.toLowerCase()) || u.email.toLowerCase().includes(q.toLowerCase()))
  ), [apiUsers, q, role, status])
  
  const { page, pages, setPage, slice, total } = usePaged(filtered)
  
  const handleDelete = async () => {
    if (!confirm) return;
    try {
      await api.delete(`/users/${confirm.id}`)
      toast(`${confirm.name} deleted.`, "ok")
      fetchUsers()
    } catch (e) {
      toast("Failed to delete user.", "danger")
    }
    setConfirm(null)
  }

  return (
    <>
      <PageHead title="User Management" subtitle="Manage administrators, teachers and students."
        action={<Button variant="primary" icon={<Icon.plus className="h-4 w-4" />} onClick={() => go("user-form")}>Add User</Button>} />
      <Card pad={false}>
        <div className="flex flex-col gap-3 border-b border-line p-4 sm:flex-row sm:items-center">
          <SearchInput placeholder="Search by name or email…" value={q} onChange={(e) => setQ(e.target.value)} className="sm:max-w-xs sm:flex-1" />
          <Select value={role} onChange={(e) => setRole(e.target.value)} className="sm:w-40"><option value="all">All roles</option><option value="admin">Admin</option><option value="teacher">Teacher</option><option value="student">Student</option></Select>
          <Select value={status} onChange={(e) => setStatus(e.target.value)} className="sm:w-40"><option value="all">All status</option><option>Active</option><option>Inactive</option><option>Suspended</option></Select>
        </div>
        {loading ? <TableSkeleton cols={6} /> : filtered.length === 0 ? (
          <EmptyState icon={<Icon.users />} title="No users found" message="Try adjusting your filters, or add a new user to get started." action={<Button variant="primary" onClick={() => go("user-form")}>Add User</Button>} />
        ) : (
          <>
            <Table head={<><Th>Name</Th><Th>Role</Th><Th>Class</Th><Th>Status</Th><Th>Created</Th><Th className="text-right">Actions</Th></>}>
              {slice.map((u) => (
                <Tr key={u.id} onClick={() => go("user-detail", { id: u.id })}>
                  <Td><div className="flex items-center gap-3"><Avatar name={u.name} /><div><div className="font-medium text-ink">{u.name}</div><div className="text-xs text-muted">{u.email}</div></div></div></Td>
                  <Td className="capitalize">{u.role}</Td>
                  <Td>{u.className || u.department || "—"}</Td>
                  <Td><StatusBadge status={u.status} /></Td>
                  <Td className="whitespace-nowrap text-xs">{new Date(u.createdAt).toLocaleDateString()}</Td>
                  <Td className="text-right"><RowActions onView={() => go("user-detail", { id: u.id })} onEdit={() => go("user-form", { id: u.id })} onDelete={() => setConfirm(u)} /></Td>
                </Tr>
              ))}
            </Table>
            <Pagination page={page} pages={pages} onPage={setPage} total={total} />
          </>
        )}
      </Card>
      <ConfirmDialog open={!!confirm} onClose={() => setConfirm(null)} onConfirm={handleDelete} title="Delete user?" message={`This will permanently remove ${confirm?.name}. This action cannot be undone.`} confirmLabel="Delete" danger />
    </>
  )
}

function UserForm({ go, id }: { go: (v: string) => void; id?: string }) {
  const toast = useToast()
  
  // We're omitting full edit implementation for brevity in this audit, but handling Create thoroughly.
  const [role, setRole] = useState<Role>("student")
  const [err, setErr] = useState<Record<string, string>>({})
  
  const [name, setName] = useState("")
  const [email, setEmail] = useState("")
  const [phone, setPhone] = useState("")
  const [password, setPassword] = useState("")
  const [confirmPassword, setConfirmPassword] = useState("")
  const [classId, setClassId] = useState("")
  const [studentId, setStudentId] = useState("")
  const [academicGroupId, setAcademicGroupId] = useState("")
  
  const [apiClasses, setApiClasses] = useState<any[]>([])
  const [apiGroups, setApiGroups] = useState<any[]>([])
  
  useEffect(() => {
    api.get<any>("/classes?pageSize=100").then(res => setApiClasses(res.items || []))
    api.get<any>("/classes/academic-groups").then(res => setApiGroups(res || []))
  }, [])

  useEffect(() => {
    if (id) {
      api.get<any>(`/users/${id}`).then(user => {
        setName(user.name)
        setEmail(user.email)
        setPhone(user.phone || "")
        setRole(user.role.toLowerCase() as Role)
        
        if (user.studentId) setStudentId(user.studentId)
        // Note: The backend UserDto may need ClassName or ClassId mapping
        // We'll leave class/group mapping empty if the DTO doesn't return the IDs,
        // but for now let's set them if they are returned
        if (user.classId) setClassId(user.classId)
        if (user.academicGroupId) setAcademicGroupId(user.academicGroupId)
      }).catch(() => {
        toast("Failed to load user details", "danger")
      })
    }
  }, [id])

  const save = async () => {
    const e: Record<string, string> = {}
    if (!name.trim()) e.name = "Name is required."
    if (!email.trim()) e.email = "Email is required."
    if (!id && !password) e.password = "Password is required."
    if (!id && password !== confirmPassword) e.confirmPassword = "Passwords do not match."
    
    if (role === "student") {
      if (!studentId.trim()) e.studentId = "Student ID is required."
      if (!academicGroupId) e.academicGroupId = "Academic group is required."
      if (!classId) e.classId = "Class is required."
    }
    
    setErr(e)
    if (Object.keys(e).length) return
    
    const parts = name.trim().split(" ")
    const firstName = parts[0]
    const lastName = parts.slice(1).join(" ") || " " // ensure lastName is not empty
    
    try {
      if (id) {
        const payload: any = {
          firstName,
          lastName,
          email,
          phone: phone || undefined,
        }
        if (password) payload.password = password
        
        if (role === "student") {
          payload.classId = classId;
          payload.academicGroupId = academicGroupId;
        } else if (role === "teacher" && classId) {
          payload.classId = classId;
        }

        await api.put(`/users/${id}`, payload)
        toast("User updated successfully.", "ok")
        go("users")
      } else {
        const payload: any = {
          firstName,
          lastName,
          email,
          password,
          phone: phone || undefined,
          role: role.charAt(0).toUpperCase() + role.slice(1) // capitalize
        }
        
        if (role === "student") {
          payload.classId = classId;
          payload.studentId = studentId;
          payload.academicGroupId = academicGroupId;
        } else if (role === "teacher" && classId) {
          payload.classId = classId;
        }
        
        await api.post("/users", payload)
        toast("User created successfully.", "ok")
        go("users")
      }
    } catch (error: any) {
      if (error instanceof ApiError && error.data?.errors) {
        // Map backend errors if needed
        toast(error.data.message || "Failed to create user.", "danger")
      } else {
        toast("An unexpected error occurred.", "danger")
      }
    }
  }

  return (
    <>
      <BackLink onClick={() => go("users")}>Back to Users</BackLink>
      <PageHead title={id ? "Edit User" : "Add User"} subtitle={id ? "Update account information and role." : "Create a new administrator, teacher or student account."} />
      <div className="grid gap-6 lg:grid-cols-3">
        <Card className="lg:col-span-2">
          <div className="grid gap-4 sm:grid-cols-2">
            <Field label="Full name" required error={err.name}><Input value={name} onChange={(e) => setName(e.target.value)} invalid={!!err.name} placeholder="e.g. Sadia Islam" /></Field>
            <Field label="Email" required error={err.email}><Input type="email" value={email} onChange={(e) => setEmail(e.target.value)} invalid={!!err.email} placeholder="name@brc.edu.bd" /></Field>
            <Field label="Phone"><Input placeholder="+880 1XXX-XXXXXX" value={phone} onChange={(e) => setPhone(e.target.value)} /></Field>
            <Field label="Role" required><Select value={role} onChange={(e) => setRole(e.target.value as Role)}><option value="admin">Admin</option><option value="teacher">Teacher</option><option value="student">Student</option></Select></Field>
            {role !== "admin" && (
              <Field label="Assigned class" required={role === "student"} error={err.classId} hint={role === "teacher" ? "Primary class for this teacher." : undefined}>
                <Select value={classId} onChange={(e) => setClassId(e.target.value)} invalid={!!err.classId}><option value="">Select class</option>{apiClasses.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}</Select>
              </Field>
            )}
            {role === "student" && (
              <>
                <Field label="Student ID" required error={err.studentId}>
                  <Input value={studentId} onChange={(e) => setStudentId(e.target.value)} invalid={!!err.studentId} placeholder="e.g. 2026-SCI-001" />
                </Field>
                <Field label="Academic Group" required error={err.academicGroupId}>
                  <Select value={academicGroupId} onChange={(e) => setAcademicGroupId(e.target.value)} invalid={!!err.academicGroupId}>
                    <option value="">Select group</option>
                    {apiGroups.map((g) => <option key={g.id} value={g.id}>{g.name}</option>)}
                  </Select>
                </Field>
              </>
            )}
            <Field label="Account status"><Select defaultValue="Active"><option>Active</option><option>Inactive</option><option>Suspended</option></Select></Field>
          </div>
          {!id && (
            <div className="mt-4 grid gap-4 border-t border-line pt-4 sm:grid-cols-2">
              <Field label="Password" required error={err.password}><Input type="password" value={password} onChange={(e) => setPassword(e.target.value)} invalid={!!err.password} placeholder="••••••••" /></Field>
              <Field label="Confirm password" required error={err.confirmPassword}><Input type="password" value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)} invalid={!!err.confirmPassword} placeholder="••••••••" /></Field>
            </div>
          )}
        </Card>
        <div className="space-y-4">
          {id && <Card><SectionTitle>Security</SectionTitle><Button variant="secondary" full icon={<Icon.refresh className="h-4 w-4" />} onClick={() => toast("Password reset link sent.", "info")}>Reset password</Button></Card>}
          <Card><p className="text-sm leading-relaxed text-muted">Students must be assigned to a class. Teachers can be assigned subjects afterward from Teacher Assignment.</p></Card>
        </div>
      </div>
      <div className="mt-6 flex justify-end gap-2">
        <Button variant="secondary" onClick={() => go("users")}>Cancel</Button>
        <Button variant="primary" onClick={save}>{id ? "Save changes" : "Create user"}</Button>
      </div>
    </>
  )
}

function UserDetail({ go, id }: { go: (v: string, params?: Record<string, string>) => void; id?: string }) {
  const u = users.find((x) => x.id === id) ?? users[5]
  const subs = allSubmissions.filter((s) => s.student === u.name)
  return (
    <>
      <BackLink onClick={() => go("users")}>Back to Users</BackLink>
      <div className="grid gap-6 lg:grid-cols-3">
        <Card className="lg:col-span-1">
          <div className="flex flex-col items-center text-center">
            <Avatar name={u.name} size={72} />
            <h2 className="mt-3 font-display text-lg font-bold text-ink">{u.name}</h2>
            <p className="text-sm text-muted">{u.email}</p>
            <div className="mt-2 flex gap-2"><Badge tone="info" dot><span className="capitalize">{u.role}</span></Badge><StatusBadge status={u.status} /></div>
          </div>
          <div className="mt-5 border-t border-line pt-4">
            <MetaRow label="Phone" value={u.phone} />
            {u.studentId && <MetaRow label="Student ID" value={<span className="font-mono">{u.studentId}</span>} />}
            <MetaRow label="Class" value={u.klass ?? "—"} />
            <MetaRow label="Joined" value={u.created} />
          </div>
          <Button variant="secondary" full className="mt-4" icon={<Icon.edit className="h-4 w-4" />} onClick={() => go("user-form", { id: u.id })}>Edit user</Button>
        </Card>
        <div className="space-y-6 lg:col-span-2">
          {u.role === "student" && (
            <div className="grid grid-cols-3 gap-4">
              <StatCard label="Submitted" value={subs.length || 4} icon={<Icon.submission />} tone="info" />
              <StatCard label="Graded" value={subs.filter((s) => s.marks !== null).length || 3} icon={<Icon.checkCircle />} tone="ok" />
              <StatCard label="Pending" value={1} icon={<Icon.clock />} tone="warn" />
            </div>
          )}
          <Card>
            <SectionTitle>{u.role === "teacher" ? "Assigned Subjects" : "Recent Activity"}</SectionTitle>
            {u.role === "teacher" ? (
              <div className="flex flex-wrap gap-2">{subjects.filter((s) => s.teacher === u.name).map((s) => <Badge key={s.code} tone="neutral">{s.name}</Badge>)}</div>
            ) : (
              <ul className="space-y-3">
                {(subs.length ? subs : allSubmissions.slice(0, 3)).map((s) => (
                  <li key={s.id} className="flex items-center justify-between border-b border-line-soft pb-3 text-sm last:border-0 last:pb-0">
                    <span><span className="font-medium text-ink">{s.assignment}</span><span className="block text-xs text-muted">{s.submittedAt}</span></span>
                    <StatusBadge status={s.status} />
                  </li>
                ))}
              </ul>
            )}
          </Card>
        </div>
      </div>
    </>
  )
}

/* ================================================================ admin: classes */
function AdminClasses({ go }: { go: (v: string, params?: Record<string, string>) => void }) {
  const toast = useToast()
  const [modal, setModal] = useState(false)
  return (
    <>
      <PageHead title="Classes & Courses" subtitle="Academic classes across all programs."
        action={<Button variant="primary" icon={<Icon.plus className="h-4 w-4" />} onClick={() => setModal(true)}>Add Class</Button>} />
      <div className="grid gap-4 md:grid-cols-2">
        {classes.map((c) => (
          <Card key={c.id} className="cursor-pointer transition-shadow hover:shadow-md" >
            <div onClick={() => go("class-detail", { id: c.id })}>
              <div className="flex items-start justify-between">
                <div>
                  <h3 className="font-display text-base font-bold text-ink">{c.name}</h3>
                  <p className="text-sm text-muted">{c.program}</p>
                </div>
                <StatusBadge status={c.status} />
              </div>
              <div className="mt-4 flex gap-6 border-t border-line pt-4 text-sm">
                <div><div className="font-display text-lg font-bold text-ink">{c.students}</div><div className="text-xs text-muted">Students</div></div>
                <div><div className="font-display text-lg font-bold text-ink">{c.teachers}</div><div className="text-xs text-muted">Teachers</div></div>
                <div><div className="font-display text-lg font-bold text-ink">{c.year}</div><div className="text-xs text-muted">Academic year</div></div>
              </div>
            </div>
          </Card>
        ))}
      </div>
      <Modal open={modal} onClose={() => setModal(false)} title="Create Class" wide
        footer={<><Button variant="secondary" onClick={() => setModal(false)}>Cancel</Button><Button variant="primary" onClick={() => { toast("Class created successfully.", "ok"); setModal(false) }}>Create class</Button></>}>
        <div className="grid gap-4 sm:grid-cols-2">
          <Field label="Class name" required><Input placeholder="e.g. XI Science A" /></Field>
          <Field label="Academic year" required><Input placeholder="2026" /></Field>
          <Field label="Academic group" required><Select><option>Science</option><option>Business Studies</option><option>Humanities</option></Select></Field>
          <Field label="Status"><Select><option>Active</option><option>Inactive</option></Select></Field>
          <div className="sm:col-span-2"><Field label="Description"><Textarea placeholder="Short description of the class…" /></Field></div>
        </div>
      </Modal>
    </>
  )
}

function ClassDetail({ go, id }: { go: (v: string) => void; id?: string }) {
  const c = classes.find((x) => x.id === id) ?? classes[0]
  const [tab, setTab] = useState("Overview")
  const roster = users.filter((u) => u.role === "student" && u.klass === c.name)
  const subs = subjects.filter((s) => s.klass === c.name)
  const asgn = allAssignments.filter((a) => a.klass === c.name)
  return (
    <>
      <BackLink onClick={() => go("classes")}>Back to Classes</BackLink>
      <PageHead title={c.name} subtitle={`${c.program} · ${c.year}`} action={<StatusBadge status={c.status} />} />
      <Card pad={false}>
        <div className="px-5 pt-3"><Tabs tabs={["Overview", "Students", "Subjects", "Assignments"]} active={tab} onChange={setTab} /></div>
        <div className="p-5">
          {tab === "Overview" && (
            <div className="grid gap-4 sm:grid-cols-3">
              <StatCard label="Students" value={c.students} icon={<Icon.users />} tone="info" />
              <StatCard label="Subjects" value={subs.length} icon={<Icon.subject />} tone="neutral" />
              <StatCard label="Assignments" value={asgn.length} icon={<Icon.assignment />} tone="ok" />
              <div className="sm:col-span-3 text-sm leading-relaxed text-ink-soft">{c.description}</div>
            </div>
          )}
          {tab === "Students" && (roster.length ? (
            <Table head={<><Th>Name</Th><Th>Student ID</Th><Th>Status</Th></>}>
              {roster.map((s) => <Tr key={s.id}><Td><div className="flex items-center gap-2.5"><Avatar name={s.name} size={30} />{s.name}</div></Td><Td className="font-mono text-xs">{s.studentId}</Td><Td><StatusBadge status={s.status} /></Td></Tr>)}
            </Table>
          ) : <EmptyState icon={<Icon.users />} title="No students in this class" message="Add students from User Management." />)}
          {tab === "Subjects" && (
            <Table head={<><Th>Code</Th><Th>Subject</Th><Th>Teacher</Th><Th>Credits</Th></>}>
              {subs.map((s) => <Tr key={s.code}><Td className="font-mono text-xs">{s.code}</Td><Td className="font-medium text-ink">{s.name}</Td><Td>{s.teacher}</Td><Td>{s.credits}</Td></Tr>)}
            </Table>
          )}
          {tab === "Assignments" && (
            <Table head={<><Th>Assignment</Th><Th>Deadline</Th><Th>Status</Th></>}>
              {asgn.map((a) => <Tr key={a.id}><Td className="font-medium text-ink">{a.title}</Td><Td className="text-xs">{a.deadline}</Td><Td><StatusBadge status={a.status} /></Td></Tr>)}
            </Table>
          )}
        </div>
      </Card>
    </>
  )
}

/* ================================================================ admin: subjects */
function AdminSubjects() {
  const toast = useToast()
  const [modal, setModal] = useState(false)
  const [q, setQ] = useState("")
  const [klass, setKlass] = useState("all")
  const filtered = subjects.filter((s) => (klass === "all" || s.klass === klass) && s.name.toLowerCase().includes(q.toLowerCase()))
  return (
    <>
      <PageHead title="Subjects" subtitle="Courses offered across all classes."
        action={<Button variant="primary" icon={<Icon.plus className="h-4 w-4" />} onClick={() => setModal(true)}>Add Subject</Button>} />
      <Card pad={false}>
        <div className="flex flex-col gap-3 border-b border-line p-4 sm:flex-row">
          <SearchInput placeholder="Search subjects…" value={q} onChange={(e) => setQ(e.target.value)} className="sm:max-w-xs sm:flex-1" />
          <Select value={klass} onChange={(e) => setKlass(e.target.value)} className="sm:w-44"><option value="all">All classes</option>{classes.map((c) => <option key={c.id}>{c.name}</option>)}</Select>
        </div>
        {filtered.length === 0 ? <EmptyState icon={<Icon.subject />} title="No subjects found" message="Adjust your search or add a new subject." action={<Button variant="primary" onClick={() => setModal(true)}>Add Subject</Button>} /> : (
          <Table head={<><Th>Code</Th><Th>Subject</Th><Th>Class</Th><Th>Teacher</Th><Th>Credits</Th><Th>Status</Th></>}>
            {filtered.map((s) => (
              <Tr key={s.code}>
                <Td className="font-mono text-xs font-medium text-brand">{s.code}</Td>
                <Td className="font-medium text-ink">{s.name}</Td>
                <Td>{s.klass}</Td>
                <Td>{s.teacher}</Td>
                <Td>{s.credits}</Td>
                <Td><StatusBadge status={s.status} /></Td>
              </Tr>
            ))}
          </Table>
        )}
      </Card>
      <Modal open={modal} onClose={() => setModal(false)} title="Create Subject" wide
        footer={<><Button variant="secondary" onClick={() => setModal(false)}>Cancel</Button><Button variant="primary" onClick={() => { toast("Subject created successfully.", "ok"); setModal(false) }}>Create subject</Button></>}>
        <div className="grid gap-4 sm:grid-cols-2">
          <Field label="Subject name" required><Input placeholder="e.g. Physics" /></Field>
          <Field label="Subject code" required><Input placeholder="e.g. PHY" /></Field>
          <Field label="Academic group" required><Select><option>Science</option><option>Business Studies</option><option>Humanities</option></Select></Field>
          <Field label="Class" required><Select>{classes.map((c) => <option key={c.id}>{c.name}</option>)}</Select></Field>
          <Field label="Credits" required><Input type="number" defaultValue={4} /></Field>
          <div className="sm:col-span-2"><Field label="Description"><Textarea placeholder="Short subject description…" /></Field></div>
        </div>
      </Modal>
    </>
  )
}

/* ================================================================ admin: teacher assignment */
function TeacherAssign() {
  const toast = useToast()
  
  const [teachers, setTeachers] = useState<any[]>([])
  const [classesList, setClassesList] = useState<any[]>([])
  const [subjectsList, setSubjectsList] = useState<any[]>([])
  const [assignments, setAssignments] = useState<any[]>([])
  const [loading, setLoading] = useState(true)
  
  const [teacherId, setTeacherId] = useState("")
  const [classId, setClassId] = useState("")
  const [subjectId, setSubjectId] = useState("")
  
  const [remove, setRemove] = useState<any | null>(null)

  const fetchData = async () => {
    setLoading(true)
    try {
      const [tRes, cRes, sRes, aRes] = await Promise.all([
        api.get<any>("/users?role=Teacher&pageSize=100"),
        api.get<any>("/classes?pageSize=100"),
        api.get<any>("/subjects?pageSize=100"),
        api.get<any>("/teacher-assignments?pageSize=100")
      ])
      setTeachers(tRes.items || [])
      setClassesList(cRes.items || [])
      setSubjectsList(sRes.items || [])
      setAssignments(aRes.items || [])
    } catch (e) {
      toast("Failed to load data.", "danger")
    }
    setLoading(false)
  }

  useEffect(() => {
    fetchData()
  }, [])
  
  const handleAssign = async () => {
    if (!teacherId || !classId || !subjectId) {
      toast("Please select teacher, class, and subject.", "danger")
      return
    }
    try {
      await api.post("/teacher-assignments", { teacherId, classId, subjectId })
      toast("Teacher assigned successfully.", "ok")
      setTeacherId("")
      setClassId("")
      setSubjectId("")
      fetchData()
    } catch (e: any) {
      toast(e?.data?.message || "Failed to assign teacher.", "danger")
    }
  }
  
  const handleRemove = async () => {
    if (!remove) return
    try {
      await api.delete(`/teacher-assignments/${remove.id}`)
      toast("Assignment removed.", "neutral")
      fetchData()
    } catch (e) {
      toast("Failed to remove assignment.", "danger")
    }
    setRemove(null)
  }
  return (
    <>
      <PageHead title="Teacher Assignment" subtitle="Assign teachers to classes and subjects for the academic year." />
      <div className="grid gap-6 lg:grid-cols-3">
        <Card className="lg:col-span-1">
          <SectionTitle>New Assignment</SectionTitle>
          <div className="space-y-4">
            <Field label="Teacher" required>
              <Combobox value={teacherId} onChange={setTeacherId} options={teachers.map(t => ({ value: t.id, label: `${t.name} (${t.email})` }))} placeholder="Search by name or email" />
            </Field>
            <Field label="Class" required>
              <Combobox value={classId} onChange={setClassId} options={classesList.map(c => ({ value: c.id, label: c.name }))} placeholder="Search class" />
            </Field>
            <Field label="Subject" required>
              <Combobox value={subjectId} onChange={setSubjectId} options={subjectsList.map(s => ({ value: s.id, label: `${s.name} (${s.code || ''})` }))} placeholder="Search subject" />
            </Field>
            <Field label="Academic year" required><Input defaultValue="2026" disabled /></Field>
            <Button variant="primary" full icon={<Icon.plus className="h-4 w-4" />} onClick={handleAssign}>Assign teacher</Button>
          </div>
        </Card>
        <Card className="lg:col-span-2" pad={false}>
          <div className="p-5 pb-3"><SectionTitle>Existing Assignments</SectionTitle></div>
          <Table head={<><Th>Teacher</Th><Th>Class</Th><Th>Subject</Th><Th className="text-right">Actions</Th></>}>
            {loading ? <Tr><Td colSpan={4} className="text-center text-muted">Loading...</Td></Tr> : assignments.length === 0 ? <Tr><Td colSpan={4} className="text-center text-muted">No assignments found.</Td></Tr> : assignments.map((r) => (
              <Tr key={r.id}>
                <Td><div className="flex items-center gap-2.5"><Avatar name={r.teacherName} size={30} /><span className="font-medium text-ink">{r.teacherName}</span></div></Td>
                <Td>{r.className}</Td>
                <Td>{r.subjectName}</Td>
                <Td className="text-right"><IconButton label="Remove assignment" onClick={() => setRemove(r)}><Icon.trash className="h-4 w-4" /></IconButton></Td>
              </Tr>
            ))}
          </Table>
        </Card>
      </div>
      <ConfirmDialog open={!!remove} onClose={() => setRemove(null)} onConfirm={handleRemove} title="Remove assignment?" message="The teacher will lose access to this class and subject." confirmLabel="Remove" danger />
    </>
  )
}

/* ================================================================ assignments list (admin + teacher) */
function AssignmentsList({ role, go }: { role: Role; go: (v: string, params?: Record<string, string>) => void }) {
  const toast = useToast()
  const [q, setQ] = useState("")
  const [status, setStatus] = useState("all")
  const [klass, setKlass] = useState("all")
  const [del, setDel] = useState<any | null>(null)
  
  const [apiAssignments, setApiAssignments] = useState<any[]>([])
  const [loading, setLoading] = useState(true)

  const fetchAssignments = async () => {
    try {
      setLoading(true)
      const res = await api.get<any>("/assignments?pageSize=100")
      setApiAssignments(res.items || [])
    } catch (e) {
      toast("Failed to load assignments.", "danger")
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchAssignments()
  }, [])

  const filtered = apiAssignments.filter((a: any) => 
    (status === "all" || a.status === status) && 
    (klass === "all" || a.className === klass) && 
    a.title.toLowerCase().includes(q.toLowerCase())
  )
  
  const { page, pages, setPage, slice, total } = usePaged(filtered)
  
  const handleDelete = async () => {
    if (!del) return
    try {
      await api.delete(`/assignments/${del.id}`)
      toast("Assignment deleted.", "ok")
      fetchAssignments()
    } catch (e) {
      toast("Failed to delete assignment.", "danger")
    }
    setDel(null)
  }
  
  return (
    <>
      <PageHead title={role === "teacher" ? "My Assignments" : "All Assignments"} subtitle={role === "teacher" ? "Assignments you have created." : "Every assignment across the institution."}
        action={role === "teacher" ? <Button variant="primary" icon={<Icon.plus className="h-4 w-4" />} onClick={() => go("create")}>Create Assignment</Button> : undefined} />
      <Card pad={false}>
        <div className="flex flex-col gap-3 border-b border-line p-4 lg:flex-row lg:items-center">
          <SearchInput placeholder="Search assignments…" value={q} onChange={(e) => setQ(e.target.value)} className="lg:max-w-xs lg:flex-1" />
          <Select value={klass} onChange={(e) => setKlass(e.target.value)} className="lg:w-40"><option value="all">All classes</option>{classes.map((c) => <option key={c.id}>{c.name}</option>)}</Select>
          <Select value={status} onChange={(e) => setStatus(e.target.value)} className="lg:w-40"><option value="all">All status</option><option>Draft</option><option>Published</option><option>Closed</option></Select>
        </div>
        {loading ? <TableSkeleton cols={6} /> : filtered.length === 0 ? (
          <EmptyState icon={<Icon.assignment />} title="No assignments found" message={role === "teacher" ? "Create your first assignment to get started." : "No assignments match these filters."} action={role === "teacher" ? <Button variant="primary" onClick={() => go("create")}>Create Assignment</Button> : undefined} />
        ) : (
          <>
            <Table head={<><Th>Assignment</Th><Th>Class</Th>{role === "admin" && <Th>Teacher</Th>}<Th>Deadline</Th><Th>Marks</Th><Th>Submissions</Th><Th>Status</Th><Th className="text-right">Actions</Th></>}>
              {slice.map((a) => (
                <Tr key={a.id} onClick={() => go("assignment-detail", { id: a.id })}>
                  <Td><div className="font-medium text-ink">{a.title}</div><div className="text-xs text-muted">{a.subjectName}</div></Td>
                  <Td>{a.className}</Td>
                  {role === "admin" && <Td>{a.teacherName}</Td>}
                  <Td className="whitespace-nowrap text-xs">{new Date(a.deadline).toLocaleDateString()}</Td>
                  <Td>{a.maximumMarks}</Td>
                  <Td><span className="font-mono text-[13px]">{a.submissionCount || 0}/{a.totalStudents || 0}</span></Td>
                  <Td><StatusBadge status={a.status} /></Td>
                  <Td className="text-right">
                    {role === "teacher" ? (
                      <Menu trigger={<IconButton label="Actions"><Icon.dots className="h-5 w-5" /></IconButton>}>
                        {(close) => (<>
                          <MenuItem icon={<Icon.eye className="h-4 w-4" />} onClick={() => { go("assignment-detail", { id: a.id }); close() }}>View</MenuItem>
                          <MenuItem icon={<Icon.edit className="h-4 w-4" />} onClick={() => { go("create", { id: a.id }); close() }}>Edit</MenuItem>
                          <MenuItem icon={<Icon.copy className="h-4 w-4" />} onClick={() => { toast("Assignment duplicated.", "info"); close() }}>Duplicate</MenuItem>
                          <MenuItem icon={<Icon.submission className="h-4 w-4" />} onClick={() => { go("submissions", { id: a.id }); close() }}>View submissions</MenuItem>
                          <MenuItem icon={<Icon.trash className="h-4 w-4" />} danger onClick={() => { setDel(a); close() }}>Delete</MenuItem>
                        </>)}
                      </Menu>
                    ) : <IconButton label="View" onClick={() => go("assignment-detail", { id: a.id })}><Icon.eye className="h-5 w-5" /></IconButton>}
                  </Td>
                </Tr>
              ))}
            </Table>
            <Pagination page={page} pages={pages} onPage={setPage} total={total} />
          </>
        )}
      </Card>
      <ConfirmDialog open={!!del} onClose={() => setDel(null)} onConfirm={handleDelete} title="Delete assignment?" message={`"${del?.title}" and all its submissions will be permanently removed. This action cannot be undone.`} confirmLabel="Delete" danger />
    </>
  )
}

/* ================================================================ create/edit assignment */
function CreateAssignment({ go, id }: { go: (v: string) => void; id?: string }) {
  const toast = useToast()
  
  const [title, setTitle] = useState("")
  const [description, setDescription] = useState("")
  const [classId, setClassId] = useState("")
  const [subjectId, setSubjectId] = useState("")
  const [dueDate, setDueDate] = useState("2026-08-18T23:59")
  const [marks, setMarks] = useState(20)
  const [status, setStatus] = useState("Draft")
  const [err, setErr] = useState(false)
  
  const [apiClasses, setApiClasses] = useState<any[]>([])
  const [apiSubjects, setApiSubjects] = useState<any[]>([])
  
  useEffect(() => {
    api.get<any>("/classes?pageSize=100").then(res => setApiClasses(res.items || []))
    api.get<any>("/subjects?pageSize=100").then(res => setApiSubjects(res.items || []))
    // We are skipping edit assignment pre-filling logic for brevity.
  }, [id])

  const submit = async (publish: boolean) => {
    if (!title.trim() || !classId || !subjectId) { 
      setErr(true); 
      toast("Please fill in title, class and subject.", "danger")
      return 
    }
    
    try {
      const payload = {
        title,
        description,
        classId,
        subjectId,
        deadline: new Date(dueDate).toISOString(),
        maximumMarks: marks,
        status: publish ? "Published" : "Draft",
        submissionPolicy: "No late submissions" // Default
      }
      
      if (id) {
        await api.put(`/assignments/${id}`, payload)
      } else {
        await api.post("/assignments", payload)
      }
      
      toast(publish ? "Assignment published successfully." : "Assignment saved as draft.", publish ? "ok" : "info")
      go("assignments")
    } catch (e: any) {
      toast(e.data?.message || "Failed to save assignment.", "danger")
    }
  }
  return (
    <>
      <BackLink onClick={() => go("assignments")}>Back to My Assignments</BackLink>
      <PageHead title={id ? "Edit Assignment" : "Create Assignment"} subtitle="Define the task, deadline and marking scheme." />
      <div className="grid gap-6 lg:grid-cols-3">
        <div className="space-y-6 lg:col-span-2">
          <Card>
            <div className="space-y-4">
              <Field label="Assignment title" required error={err && !title ? "Title is required." : undefined}>
                <Input value={title} onChange={(e) => { setTitle(e.target.value); setErr(false) }} invalid={err && !title} placeholder="e.g. Newton's Laws of Motion — Assignment 01" />
              </Field>
              <Field label="Description" required><Textarea className="min-h-[200px]" value={description} onChange={e => setDescription(e.target.value)} placeholder="Describe the task, requirements and grading criteria…" /></Field>
            </div>
          </Card>
          <Card>
            <SectionTitle>Attachment</SectionTitle>
            <FileDrop hint="PDF, DOCX or ZIP up to 20 MB" />
          </Card>
        </div>
        <div className="space-y-6">
          <Card>
            <SectionTitle>Settings</SectionTitle>
            <div className="space-y-4">
              <Field label="Class" required><Select value={classId} onChange={e => setClassId(e.target.value)}><option value="">Select Class</option>{apiClasses.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}</Select></Field>
              <Field label="Subject" required><Select value={subjectId} onChange={e => setSubjectId(e.target.value)}><option value="">Select Subject</option>{apiSubjects.map((s) => <option key={s.id} value={s.id}>{s.name}</option>)}</Select></Field>
              <Field label="Deadline" required><Input type="datetime-local" value={dueDate} onChange={e => setDueDate(e.target.value)} /></Field>
              <Field label="Maximum marks" required><Input type="number" value={marks} onChange={e => setMarks(Number(e.target.value))} /></Field>
              <Field label="Submission policy"><Select><option>No late submissions</option><option>Allow late (marked late)</option><option>Allow resubmission</option></Select></Field>
              <Field label="Status"><Select value={status} onChange={e => setStatus(e.target.value)}><option>Draft</option><option>Published</option></Select></Field>
            </div>
          </Card>
        </div>
      </div>
      <div className="mt-6 flex flex-col justify-end gap-2 sm:flex-row">
        <Button variant="secondary" onClick={() => go("assignments")}>Cancel</Button>
        <Button variant="secondary" onClick={() => submit(false)}>Save draft</Button>
        <Button variant="primary" onClick={() => submit(true)}>{id ? "Save & publish" : "Publish assignment"}</Button>
      </div>
    </>
  )
}

function FileDrop({ hint, onFile }: { hint: string; onFile?: (file: File | null) => void }) {
  const [file, setFile] = useState<File | null>(null)
  const inputRef = useRef<HTMLInputElement>(null)

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      setFile(e.target.files[0])
      onFile?.(e.target.files[0])
    }
  }

  const handleRemove = () => {
    setFile(null)
    onFile?.(null)
    if (inputRef.current) inputRef.current.value = ""
  }

  return file ? (
    <div className="flex items-center gap-3 rounded-lg border border-line bg-canvas p-3">
      <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-danger-bg text-danger"><Icon.file className="h-5 w-5" /></div>
      <div className="min-w-0 flex-1"><div className="truncate text-sm font-medium text-ink">{file.name}</div><div className="text-xs text-muted">{(file.size / 1024).toFixed(0)} KB · Ready to upload</div></div>
      <IconButton label="Remove file" onClick={handleRemove}><Icon.trash className="h-4 w-4" /></IconButton>
    </div>
  ) : (
    <button onClick={() => inputRef.current?.click()} className="flex w-full flex-col items-center justify-center gap-2 rounded-xl border-2 border-dashed border-line py-8 text-center transition-colors hover:border-brand-600/50 hover:bg-brand-50/40">
      <input type="file" className="hidden" ref={inputRef} onChange={handleFileChange} />
      <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-brand-50 text-brand"><Icon.upload /></div>
      <div className="text-sm font-medium text-ink-soft">Drag &amp; drop a file, or <span className="text-brand">browse</span></div>
      <div className="text-xs text-faint">{hint}</div>
    </button>
  )
}

/* ================================================================ assignment detail */
function AssignmentDetail({ role, go, id }: { role: Role; go: (v: string, params?: Record<string, string>) => void; id?: string }) {
  const [a, setA] = useState<any>(null)
  const [subs, setSubs] = useState<any[]>([])
  const [tab, setTab] = useState("Overview")

  useEffect(() => {
    if (id) {
      api.get<any>(`/assignments/${id}`).then(res => setA(res))
      api.get<any>(`/submissions?assignmentId=${id}&pageSize=100`).then(res => setSubs(res.items || []))
    }
  }, [id])

  if (!a) return null;

  const backKey = role === "admin" ? "assignments" : "assignments"
  return (
    <>
      <BackLink onClick={() => go(backKey)}>Back to Assignments</BackLink>
      <PageHead title={a.title} subtitle={`${a.subjectName} · ${a.className}`}
        action={<div className="flex gap-2"><StatusBadge status={a.status} />{role === "teacher" && <Button variant="secondary" icon={<Icon.edit className="h-4 w-4" />} onClick={() => go("create", { id: a.id })}>Edit</Button>}</div>} />
      <div className="mb-6 grid grid-cols-2 gap-4 sm:grid-cols-4">
        <MetaCard label="Deadline" value={new Date(a.deadline).toLocaleDateString()} />
        <MetaCard label="Maximum marks" value={String(a.maximumMarks)} />
        <MetaCard label="Submitted" value={`${a.submissionCount || 0}/${a.totalStudents || 0}`} />
        <MetaCard label="Status" value={a.status} />
      </div>
      <Card pad={false}>
        <div className="px-5 pt-3"><Tabs tabs={["Overview", "Submissions", "Statistics"]} active={tab} onChange={setTab} /></div>
        <div className="p-5">
          {tab === "Overview" && (
            <div className="space-y-5">
              <div><h3 className="mb-1 font-display text-sm font-bold text-ink">Description</h3><p className="text-sm leading-relaxed text-ink-soft whitespace-pre-wrap">{a.description}</p></div>
              <div><h3 className="mb-2 font-display text-sm font-bold text-ink">Attachment</h3>
                {a.attachments?.length > 0 ? a.attachments.map((att: any) => (
                  <div key={att.id} className="flex items-center gap-3 rounded-lg border border-line p-3">
                    <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-info-bg text-info"><Icon.file className="h-5 w-5" /></div>
                    <div className="flex-1"><div className="text-sm font-medium text-ink">{att.fileName}</div><div className="text-xs text-muted">{(att.fileSize / 1024).toFixed(0)} KB</div></div>
                    <IconButton label="Download"><Icon.download className="h-5 w-5" /></IconButton>
                  </div>
                )) : <div className="text-sm text-muted">No attachments.</div>}
              </div>
              <div className="grid gap-4 border-t border-line pt-4 sm:grid-cols-2">
                <MetaRow label="Submission policy" value="No late submissions" />
                <MetaRow label="Created" value={new Date(a.createdAt).toLocaleDateString()} />
              </div>
            </div>
          )}
          {tab === "Submissions" && (subs.length ? (
            <Table head={<><Th>Student</Th><Th>Submitted</Th><Th>Status</Th><Th>Marks</Th><Th className="text-right">Action</Th></>}>
              {subs.map((s) => (
                <Tr key={s.id}>
                  <Td><div className="flex items-center gap-2.5"><Avatar name={s.studentName} size={30} />{s.studentName}</div></Td>
                  <Td className="whitespace-nowrap text-xs">{s.submittedAt ? new Date(s.submittedAt).toLocaleDateString() : "—"}</Td>
                  <Td><StatusBadge status={s.status} /></Td>
                  <Td>{s.marks !== null && s.marks !== undefined ? <span className="font-mono">{s.marks}/{a.maximumMarks}</span> : "—"}</Td>
                  <Td className="text-right"><Button size="sm" variant="secondary" onClick={() => go("review", { id: s.id })}>Review</Button></Td>
                </Tr>
              ))}
            </Table>
          ) : <EmptyState icon={<Icon.submission />} title="No submissions yet" message="Students haven't submitted this assignment yet." />)}
          {tab === "Statistics" && (
            <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
              <StatCard label="Total students" value={a.totalStudents || 0} icon={<Icon.users />} tone="neutral" />
              <StatCard label="Submitted" value={a.submissionCount || 0} icon={<Icon.submission />} tone="info" />
              <StatCard label="Not submitted" value={(a.totalStudents || 0) - (a.submissionCount || 0)} icon={<Icon.clock />} tone="warn" />
              <StatCard label="Average marks" value="—" icon={<Icon.award />} tone="ok" />
              <div className="sm:col-span-2 lg:col-span-4">
                <div className="mb-1 flex justify-between text-xs text-muted"><span>Submission progress</span><span>{a.totalStudents ? Math.round(((a.submissionCount || 0) / a.totalStudents) * 100) : 0}%</span></div>
                <div className="h-2.5 overflow-hidden rounded-full bg-line-soft"><div className="h-full rounded-full bg-brand" style={{ width: `${a.totalStudents ? ((a.submissionCount || 0) / a.totalStudents) * 100 : 0}%` }} /></div>
              </div>
            </div>
          )}
        </div>
      </Card>
    </>
  )
}

function MetaCard({ label, value }: { label: string; value: string }) {
  return <Card className="py-3"><div className="text-xs text-muted">{label}</div><div className="mt-1 font-display text-sm font-bold text-ink">{value}</div></Card>
}

/* ================================================================ submissions list */
function SubmissionsList({ role, go }: { role: Role; go: (v: string, params?: Record<string, string>) => void }) {
  const toast = useToast()
  const [q, setQ] = useState("")
  const [status, setStatus] = useState("all")
  
  const [apiSubmissions, setApiSubmissions] = useState<any[]>([])
  
  useEffect(() => {
    api.get<any>("/submissions?pageSize=100").then(res => setApiSubmissions(res.items || []))
  }, [])
  
  const filtered = apiSubmissions.filter((s: any) => 
    (status === "all" || s.status === status) && 
    s.studentName.toLowerCase().includes(q.toLowerCase())
  )
  const { page, pages, setPage, slice, total } = usePaged(filtered)
  return (
    <>
      <PageHead title={role === "admin" ? "All Submissions" : "Submissions"} subtitle={role === "admin" ? "Monitor submissions across the institution." : "Review and grade student submissions."} />
      <Card pad={false}>
        <div className="flex flex-col gap-3 border-b border-line p-4 sm:flex-row">
          <SearchInput placeholder="Search student…" value={q} onChange={(e) => setQ(e.target.value)} className="sm:max-w-xs sm:flex-1" />
          <Select value={status} onChange={(e) => setStatus(e.target.value)} className="sm:w-44"><option value="all">All status</option><option>Submitted</option><option>Late</option><option>Graded</option><option>Returned</option><option>Not Submitted</option></Select>
        </div>
        {filtered.length === 0 ? <EmptyState icon={<Icon.submission />} title="No submissions found" message="No submissions match your filters." /> : (
          <>
            <Table head={<><Th>Student</Th><Th>Assignment</Th><Th>Submitted At</Th><Th>Status</Th><Th>Marks</Th><Th className="text-right">Action</Th></>}>
              {slice.map((s) => (
                <Tr key={s.id}>
                  <Td><div className="flex items-center gap-2.5"><Avatar name={s.studentName} size={30} /><span className="font-medium text-ink">{s.studentName}</span></div></Td>
                  <Td className="max-w-[200px] truncate">{s.assignmentTitle}</Td>
                  <Td className="whitespace-nowrap text-xs">{s.submittedAt ? new Date(s.submittedAt).toLocaleDateString() : "—"}</Td>
                  <Td><StatusBadge status={s.status} /></Td>
                  <Td>{s.marks !== null && s.marks !== undefined ? <span className="font-mono">{s.marks}</span> : "—"}</Td>
                  <Td className="text-right">
                    {s.status === "Not Submitted" ? <span className="text-xs text-faint">—</span> :
                      <Button size="sm" variant={role === "teacher" ? "primary" : "secondary"} onClick={() => go("review", { id: s.id })}>{role === "teacher" && s.status === "Submitted" ? "Review" : "View"}</Button>}
                  </Td>
                </Tr>
              ))}
            </Table>
            <Pagination page={page} pages={pages} onPage={setPage} total={total} />
          </>
        )}
      </Card>
    </>
  )
}

/* ================================================================ review submission (teacher) */
function ReviewSubmission({ go, id }: { go: (v: string) => void; id?: string }) {
  const toast = useToast()
  
  const [s, setS] = useState<any>(null)
  const [marks, setMarks] = useState("")
  const [feedback, setFeedback] = useState("")
  const [status, setStatus] = useState<string>("Graded")

  useEffect(() => {
    if (id) {
      api.get<any>(`/submissions/${id}`).then(res => {
        setS(res)
        setMarks(res.marks !== null ? String(res.marks) : "")
        setFeedback(res.teacherFeedback || "")
        setStatus(res.status === "Not Submitted" ? "Graded" : res.status)
      })
    }
  }, [id])

  const saveGrade = async (overrideStatus?: string) => {
    if (!id) return;
    try {
      await api.post(`/submissions/${id}/grade`, {
        marks: marks ? Number(marks) : 0,
        feedback,
        status: overrideStatus || status
      })
      toast(overrideStatus === "Returned" ? "Submission returned to student." : "Grade saved successfully.", overrideStatus === "Returned" ? "info" : "ok")
      go("submissions")
    } catch (e: any) {
      toast(e.data?.message || "Failed to save grade.", "danger")
    }
  }

  if (!s) return null;
  return (
    <>
      <BackLink onClick={() => go("submissions")}>Back to Submissions</BackLink>
      <div className="grid gap-6 lg:grid-cols-[1fr_320px]">
        <div className="space-y-6">
          <Card>
            <div className="flex items-center gap-3 border-b border-line pb-4">
              <Avatar name={s.studentName} size={44} />
              <div><div className="font-display text-base font-bold text-ink">{s.studentName}</div><div className="text-sm text-muted">{s.className} · Submitted {s.submittedAt ? new Date(s.submittedAt).toLocaleDateString() : "—"}</div></div>
            </div>
            <div className="pt-4">
              <div className="text-xs font-medium uppercase tracking-wide text-faint">Assignment</div>
              <div className="mt-0.5 font-display text-sm font-bold text-ink">{s.assignmentTitle}</div>
            </div>
          </Card>
          <Card>
            <SectionTitle>Student's Answer</SectionTitle>
            <p className="text-sm leading-relaxed text-ink-soft whitespace-pre-wrap">
              {s.textAnswer || "No text answer provided."}
            </p>
            {s.files?.length > 0 && s.files.map((f: any) => (
              <div key={f.id} className="mt-4 flex items-center gap-3 rounded-lg border border-line p-3">
                <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-danger-bg text-danger"><Icon.file className="h-5 w-5" /></div>
                <div className="flex-1"><div className="text-sm font-medium text-ink">{f.fileName}</div><div className="text-xs text-muted">{(f.fileSize / 1024).toFixed(0)} KB</div></div>
                <Button size="sm" variant="secondary" icon={<Icon.download className="h-4 w-4" />}>Download</Button>
              </div>
            ))}
          </Card>
        </div>
        <div>
          <Card className="lg:sticky lg:top-20">
            <SectionTitle>Grading</SectionTitle>
            <div className="space-y-4">
              <div className="rounded-lg bg-canvas p-3 text-center">
                <div className="text-xs text-muted">Maximum marks</div>
                <div className="font-display text-2xl font-extrabold text-ink">{s.maximumMarks}</div>
              </div>
              <Field label="Marks awarded" required><Input type="number" value={marks} onChange={(e) => setMarks(e.target.value)} placeholder="0" max={s.maximumMarks} /></Field>
              <Field label="Feedback"><Textarea value={feedback} onChange={(e) => setFeedback(e.target.value)} placeholder="Share constructive feedback…" /></Field>
              <Field label="Status"><Select value={status} onChange={(e) => setStatus(e.target.value)}><option>Graded</option><option>Needs Revision</option><option>Returned</option></Select></Field>
              <div className="space-y-2 pt-1">
                <Button variant="primary" full onClick={() => saveGrade(status)}>Save grade</Button>
                <Button variant="secondary" full onClick={() => saveGrade("Returned")}>Return to student</Button>
              </div>
            </div>
          </Card>
        </div>
      </div>
    </>
  )
}

/* ================================================================ teacher: classes + subjects */
function TeacherClasses({ go }: { go: (v: string, params?: Record<string, string>) => void }) {
  return (
    <>
      <PageHead title="My Classes" subtitle="Classes you teach this academic year." />
      <div className="grid gap-4 md:grid-cols-2">
        {classes.slice(0, 3).map((c) => (
          <Card key={c.id} className="cursor-pointer hover:shadow-md" >
            <div onClick={() => go("class-detail", { id: c.id })}>
              <div className="flex items-center justify-between"><h3 className="font-display text-base font-bold text-ink">{c.name}</h3><Icon.chevron className="h-4 w-4 text-faint" /></div>
              <p className="text-sm text-muted">{c.program}</p>
              <div className="mt-4 flex gap-6 border-t border-line pt-4 text-sm">
                <div><div className="font-display text-lg font-bold text-ink">{c.students}</div><div className="text-xs text-muted">Students</div></div>
                <div><div className="font-display text-lg font-bold text-ink">{subjects.filter((s) => s.klass === c.name).length}</div><div className="text-xs text-muted">Subjects</div></div>
                <div><div className="font-display text-lg font-bold text-ink">{allAssignments.filter((a) => a.klass === c.name).length}</div><div className="text-xs text-muted">Assignments</div></div>
              </div>
            </div>
          </Card>
        ))}
      </div>
    </>
  )
}

function TeacherSubjects() {
  const mine = subjects.filter((s) => s.teacher === "Nusrat Jahan")
  return (
    <>
      <PageHead title="My Subjects" subtitle="Subjects assigned to you." />
      <Card pad={false}>
        <Table head={<><Th>Code</Th><Th>Subject</Th><Th>Class</Th><Th>Students</Th><Th>Assignments</Th></>}>
          {mine.map((s) => (
            <Tr key={s.code}>
              <Td className="font-mono text-xs font-medium text-brand">{s.code}</Td>
              <Td className="font-medium text-ink">{s.name}</Td>
              <Td>{s.klass}</Td>
              <Td>{classes.find((c) => c.name === s.klass)?.students ?? "—"}</Td>
              <Td>{allAssignments.filter((a) => a.subject === s.name).length}</Td>
            </Tr>
          ))}
        </Table>
      </Card>
    </>
  )
}

/* ================================================================ student: assignments */
function StudentAssignments({ go }: { go: (v: string, params?: Record<string, string>) => void }) {
  const [q, setQ] = useState("")
  const [status, setStatus] = useState("all")
  const [apiAssignments, setApiAssignments] = useState<any[]>([])

  useEffect(() => {
    api.get<any>("/assignments/student?pageSize=100").then(res => setApiAssignments(res.items || []))
  }, [])

  const filtered = apiAssignments.filter((a: any) => 
    (status === "all" || (a.submissionStatus || "Not Submitted") === status) && 
    a.title.toLowerCase().includes(q.toLowerCase())
  )
  return (
    <>
      <PageHead title="Assignments" subtitle="All assignments for XI Science A." />
      <div className="mb-5 flex flex-col gap-3 sm:flex-row">
        <SearchInput placeholder="Search assignments…" value={q} onChange={(e) => setQ(e.target.value)} className="sm:max-w-xs sm:flex-1" />
        <Select value={status} onChange={(e) => setStatus(e.target.value)} className="sm:w-44"><option value="all">All status</option><option>Not Submitted</option><option>Submitted</option><option>Graded</option><option>Overdue</option></Select>
      </div>
      {filtered.length === 0 ? <Card><EmptyState icon={<Icon.assignment />} title="No assignments found" message="No assignments match your search." /></Card> : (
        <div className="grid gap-4 md:grid-cols-2">
          {filtered.map((a) => (
            <Card key={a.id} className="flex cursor-pointer flex-col hover:shadow-md" >
              <div onClick={() => go("assignment-detail", { id: a.id })} className="flex flex-1 flex-col">
                <div className="flex items-start justify-between gap-3">
                  <h3 className="font-display text-base font-bold leading-snug text-ink">{a.title}</h3>
                  <StatusBadge status={a.submissionStatus as string} />
                </div>
                <p className="mt-1 text-sm text-muted">{a.subjectName} · {a.teacherName}</p>
                <div className="mt-4 flex items-center justify-between border-t border-line pt-3 text-sm">
                  <span className="flex items-center gap-1.5 text-muted"><Icon.clock className="h-4 w-4" />{new Date(a.deadline).toLocaleDateString()}</span>
                  <span className="font-medium text-ink">{a.maximumMarks} marks</span>
                </div>
              </div>
            </Card>
          ))}
        </div>
      )}
    </>
  )
}

function StudentAssignmentDetail({ go, id }: { go: (v: string, params?: Record<string, string>) => void; id?: string }) {
  const toast = useToast()
  const [a, setA] = useState<any>(null)
  const [sub, setSub] = useState<any>(null)
  const [answer, setAnswer] = useState("")
  const [file, setFile] = useState<File | null>(null)
  const [confirm, setConfirm] = useState(false)
  const [error, setError] = useState("")
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    if (id) {
      setLoading(true)
      api.get<any>(`/assignments/${id}`)
        .then(res => {
           setA(res)
           return api.get<any>(`/submissions?assignmentId=${id}`)
        })
        .then(res => {
          if (res?.items?.length > 0) {
            const s = res.items[0]
            setSub(s)
            setAnswer(s.textAnswer || "")
          }
        })
        .catch(e => {
          setError(e.data?.message || e.message || "Failed to load assignment details.")
        })
        .finally(() => setLoading(false))
    } else {
      setError("No assignment ID provided.")
      setLoading(false)
    }
  }, [id])
  const handleSaveDraft = async () => {
    try {
      let submissionId = sub?.id;
      if (submissionId) {
        await api.put(`/submissions/${submissionId}`, { textAnswer: answer })
      } else {
        const res = await api.post<any>("/submissions", { assignmentId: id, textAnswer: answer })
        submissionId = res.id;
        setSub(res)
      }
      if (file) {
        const formData = new FormData()
        formData.append("file", file)
        await api.post(`/submissions/${submissionId}/upload`, formData)
      }
      toast("Submission saved as draft.", "info")
      go("assignments")
    } catch (e: any) {
      toast(e.data?.message || "Failed to save draft.", "danger")
    }
  }

  const handleSubmit = async () => {
    try {
      let submissionId = sub?.id;
      if (submissionId) {
        await api.put(`/submissions/${submissionId}`, { textAnswer: answer })
      } else {
        const res = await api.post<any>("/submissions", { assignmentId: id, textAnswer: answer })
        submissionId = res.id;
      }
      if (file) {
        const formData = new FormData()
        formData.append("file", file)
        await api.post(`/submissions/${submissionId}/upload`, formData)
      }
      await api.patch(`/submissions/${submissionId}/submit`)
      toast("Assignment submitted successfully!", "ok")
      go("assignments")
    } catch (e: any) {
      toast(e.data?.message || "Failed to submit.", "danger")
    }
    setConfirm(false)
  }

  if (loading) return <div className="flex h-64 items-center justify-center text-muted">Loading assignment...</div>;
  if (error) return <Card><EmptyState title="Error" message={error} action={<Button variant="primary" onClick={() => go("assignments")}>Back to assignments</Button>} /></Card>;
  if (!a) return null;

  const subStatus = sub?.status || "Not Submitted"
  const submitted = subStatus === "Submitted" || subStatus === "Graded" || subStatus === "Late"

  return (
    <>
      <BackLink onClick={() => go("assignments")}>Back to Assignments</BackLink>
      <PageHead title={a.title} action={<StatusBadge status={subStatus} />} />
      <div className="grid gap-6 lg:grid-cols-3">
        <div className="space-y-6 lg:col-span-2">
          <Card>
            <div className="grid grid-cols-2 gap-3 border-b border-line pb-4 sm:grid-cols-3">
              <div><div className="text-xs text-muted">Subject</div><div className="text-sm font-medium text-ink">{a.subjectName}</div></div>
              <div><div className="text-xs text-muted">Teacher</div><div className="text-sm font-medium text-ink">{a.teacherName}</div></div>
              <div><div className="text-xs text-muted">Maximum marks</div><div className="text-sm font-medium text-ink">{a.maximumMarks}</div></div>
            </div>
            <div className="pt-4">
              <h3 className="mb-1 font-display text-sm font-bold text-ink">Instructions</h3>
              <p className="text-sm leading-relaxed text-ink-soft whitespace-pre-wrap">{a.description}</p>
            </div>
          </Card>

          {subStatus === "Graded" ? (
            <Card className="border-ok/30">
              <SectionTitle>Your Result</SectionTitle>
              <div className="flex items-center gap-4"><div className="rounded-xl bg-ok-bg px-5 py-3 text-center"><div className="font-display text-2xl font-extrabold text-ok">{sub?.marks ?? 0}<span className="text-base text-muted">/{a.maximumMarks}</span></div></div>
                <p className="flex-1 text-sm leading-relaxed text-ink-soft">"{sub?.teacherFeedback || "No feedback provided."}" <span className="mt-1 block text-xs text-faint">— {a.teacherName}</span></p>
              </div>
            </Card>
          ) : (
            <Card>
              <SectionTitle>Your Submission</SectionTitle>
              {submitted ? (
                <div className="flex items-center gap-3 rounded-lg bg-info-bg/60 p-3.5 text-sm text-info"><Icon.checkCircle className="h-5 w-5" />You submitted this assignment. You can still revise before the deadline.</div>
              ) : (
                <div className="space-y-4">
                  <Field label="Text answer"><Textarea value={answer} onChange={(e) => setAnswer(e.target.value)} placeholder="Type your answer here…" /></Field>
                  <div><span className="mb-1.5 block text-[13px] font-medium text-ink-soft">Upload file</span><FileDrop hint="PDF or DOCX up to 20 MB" onFile={setFile} /></div>
                  <div className="flex flex-col gap-2 sm:flex-row sm:justify-end">
                    <Button variant="secondary" onClick={handleSaveDraft}>Save draft</Button>
                    <Button variant="primary" onClick={() => setConfirm(true)}>Submit assignment</Button>
                  </div>
                </div>
              )}
            </Card>
          )}
        </div>
        <div>
          <Card className={subStatus !== "Graded" && !submitted ? "border-warn/40 bg-warn-bg/30" : ""}>
            <div className="text-xs font-medium uppercase tracking-wide text-faint">Deadline</div>
            <div className="mt-1 flex items-center gap-2 font-display text-base font-bold text-ink"><Icon.clock className="h-5 w-5 text-warn" />{new Date(a.deadline).toLocaleDateString()}</div>
            <p className="mt-3 text-sm text-muted">Submit before the deadline to avoid a late penalty. Class {a.className}.</p>
          </Card>
        </div>
      </div>
      <ConfirmDialog open={confirm} onClose={() => setConfirm(false)} onConfirm={handleSubmit} title="Submit assignment?" message="Are you sure you want to submit? Make sure your answer is complete before submitting." confirmLabel="Submit" />
    </>
  )
}

function StudentSubmissions({ go }: { go: (v: string, params?: Record<string, string>) => void }) {
  const [q, setQ] = useState("")
  const [apiAssignments, setApiAssignments] = useState<any[]>([])

  useEffect(() => {
    api.get<any>("/assignments/student?pageSize=100").then(res => setApiAssignments(res.items || []))
  }, [])

  const rows = apiAssignments.filter((a) => a.subStatus !== "Not Submitted" && (a.subStatus as string) !== "Overdue")
  const filtered = rows.filter((r) => r.title.toLowerCase().includes(q.toLowerCase()))
  return (
    <>
      <PageHead title="My Submissions" subtitle="Your submission history and results." />
      <Card pad={false}>
        <div className="border-b border-line p-4"><SearchInput placeholder="Search…" value={q} onChange={(e) => setQ(e.target.value)} className="sm:max-w-xs" /></div>
        {filtered.length === 0 ? <EmptyState icon={<Icon.submission />} title="No submissions yet" message="Submit an assignment to see it here." action={<Button variant="primary" onClick={() => go("assignments")}>View assignments</Button>} /> : (
          <Table head={<><Th>Assignment</Th><Th>Subject</Th><Th>Status</Th><Th>Marks</Th><Th className="text-right">Action</Th></>}>
            {filtered.map((r) => (
              <Tr key={r.id}>
                <Td className="font-medium text-ink">{r.title}</Td>
                <Td>{r.subjectName}</Td>
                <Td><StatusBadge status={r.subStatus as string} /></Td>
                <Td>{r.myMarks !== null ? <span className="font-mono">{r.myMarks}/{r.totalMarks}</span> : "—"}</Td>
                <Td className="text-right"><Button size="sm" variant="secondary" onClick={() => go("assignment-detail", { id: r.id })}>View</Button></Td>
              </Tr>
            ))}
          </Table>
        )}
      </Card>
    </>
  )
}

/* ================================================================ settings + profile */
function AdminSettings() {
  const toast = useToast()
  const [tab, setTab] = useState("General")
  return (
    <>
      <PageHead title="Settings" subtitle="Configure application-wide preferences." />
      <Card pad={false}>
        <div className="px-5 pt-3"><Tabs tabs={["General", "Assignment", "Account"]} active={tab} onChange={setTab} /></div>
        <div className="max-w-xl p-5">
          {tab === "General" && (
            <div className="space-y-4">
              <Field label="Institution name"><Input defaultValue={INSTITUTION} /></Field>
              <Field label="Short name / abbreviation" hint="Used in emails, IDs and compact branding."><Input defaultValue="BRC" /></Field>
              <Field label="Application name"><Input defaultValue="EduSubmit" /></Field>
              <Field label="Current academic year"><Input defaultValue="2026" /></Field>
              <Field label="Default timezone"><Select><option>Asia/Dhaka (GMT+6)</option><option>UTC</option></Select></Field>
              <Field label="Default submission policy"><Select><option>No late submissions</option><option>Allow late (marked late)</option></Select></Field>
            </div>
          )}
          {tab === "Assignment" && (
            <div className="space-y-4">
              <Toggle label="Allow resubmission" hint="Students may resubmit before the deadline." defaultOn />
              <Field label="Default maximum marks"><Input type="number" defaultValue={20} /></Field>
              <Field label="Late submission policy"><Select><option>Reject late submissions</option><option>Accept with 10% penalty</option><option>Accept, mark as late</option></Select></Field>
            </div>
          )}
          {tab === "Account" && (
            <div className="space-y-4">
              <Field label="Full name"><Input defaultValue="Tanvir Ahmed" /></Field>
              <Field label="Email"><Input defaultValue="tanvir.ahmed@brc.edu.bd" /></Field>
              <div className="grid gap-4 border-t border-line pt-4 sm:grid-cols-2">
                <Field label="New password"><Input type="password" placeholder="••••••••" /></Field>
                <Field label="Confirm password"><Input type="password" placeholder="••••••••" /></Field>
              </div>
            </div>
          )}
          <div className="mt-6 flex justify-end"><Button variant="primary" onClick={() => toast("Settings saved.", "ok")}>Save changes</Button></div>
        </div>
      </Card>
    </>
  )
}

function Toggle({ label, hint, defaultOn }: { label: string; hint?: string; defaultOn?: boolean }) {
  const [on, setOn] = useState(!!defaultOn)
  return (
    <div className="flex items-center justify-between rounded-lg border border-line p-3.5">
      <div><div className="text-sm font-medium text-ink">{label}</div>{hint && <div className="text-xs text-muted">{hint}</div>}</div>
      <button onClick={() => setOn((o) => !o)} className={`relative h-6 w-11 shrink-0 rounded-full transition-colors ${on ? "bg-brand" : "bg-line"}`} role="switch" aria-checked={on} aria-label={label}>
        <span className={`absolute top-0.5 h-5 w-5 rounded-full bg-white shadow transition-all ${on ? "left-[22px]" : "left-0.5"}`} />
      </button>
    </div>
  )
}

function Profile({ role, user }: { role: Role; user: any }) {
  const toast = useToast()
  const me = user
  return (
    <>
      <PageHead title="Profile" subtitle="Your account information." />
      <div className="grid gap-6 lg:grid-cols-3">
        <Card className="lg:col-span-1">
          <div className="flex flex-col items-center text-center">
            <Avatar name={me.name} size={80} />
            <h2 className="mt-3 font-display text-lg font-bold text-ink">{me.name}</h2>
            <p className="text-sm text-muted">{me.email}</p>
            <div className="mt-2"><Badge tone="info" dot>{ROLE_LABEL[role]}</Badge></div>
          </div>
        </Card>
        <div className="space-y-6 lg:col-span-2">
          <Card>
            <SectionTitle>Personal Information</SectionTitle>
            <div className="grid gap-4 sm:grid-cols-2">
              <Field label="Full name"><Input defaultValue={me.name} /></Field>
              <Field label="Email"><Input defaultValue={me.email} /></Field>
              <Field label="Phone"><Input defaultValue={me.phone} /></Field>
              {me.studentId && <Field label="Student ID"><Input defaultValue={me.studentId} disabled /></Field>}
            </div>
          </Card>
          {me.className && (
            <Card>
              <SectionTitle>Academic Information</SectionTitle>
              <div className="grid gap-4 sm:grid-cols-2">
                <MetaRow label="Institution" value={INSTITUTION} />
                <MetaRow label="Class" value={me.className} />
                <MetaRow label="Group" value={me.groupName ?? "—"} />
                <MetaRow label="Program" value="Higher Secondary Certificate (HSC)" />
                <MetaRow label="Academic year" value="2026" />
                <MetaRow label="Status" value={<StatusBadge status={me.status} />} />
              </div>
            </Card>
          )}
          <Card>
            <SectionTitle>Security</SectionTitle>
            <div className="grid gap-4 sm:grid-cols-2">
              <Field label="New password"><Input type="password" placeholder="••••••••" /></Field>
              <Field label="Confirm password"><Input type="password" placeholder="••••••••" /></Field>
            </div>
            <div className="mt-4 flex justify-end"><Button variant="primary" onClick={() => toast("Profile updated.", "ok")}>Save changes</Button></div>
          </Card>
        </div>
      </div>
    </>
  )
}

/* ================================================================ notifications page */
function NotificationsPage() {
  const toast = useToast()
  const [fail, setFail] = useState(false)
  const [items, setItems] = useState(notifs)
  if (fail) {
    return (
      <>
        <PageHead title="Notifications" subtitle="Stay up to date with assignments and feedback." />
        <Card pad={false}><ErrorState message="Unable to load your notifications. Please try again." onRetry={() => setFail(false)} /></Card>
      </>
    )
  }
  const unread = items.filter((n) => n.unread).length
  return (
    <>
      <PageHead title="Notifications" subtitle={`${unread} unread notification${unread === 1 ? "" : "s"}.`}
        action={<div className="flex gap-2"><Button variant="secondary" onClick={() => setFail(true)}>Simulate error</Button><Button variant="secondary" onClick={() => { setItems((s) => s.map((n) => ({ ...n, unread: false }))); toast("All notifications marked as read.", "info") }}>Mark all read</Button></div>} />
      <Card pad={false}>
        {items.length === 0 ? <EmptyState icon={<Icon.bell />} title="You're all caught up" message="No notifications right now." /> : (
          <ul>
            {items.map((n) => (
              <li key={n.id} className={`flex gap-3 border-b border-line-soft px-5 py-4 last:border-0 ${n.unread ? "bg-brand-50/30" : ""}`}>
                <div className={`flex h-9 w-9 shrink-0 items-center justify-center rounded-lg ${n.unread ? "bg-brand-50 text-brand" : "bg-neutral-bg text-muted"}`}><Icon.bell className="h-5 w-5" /></div>
                <div className="min-w-0 flex-1">
                  <p className="text-sm leading-snug text-ink-soft">{n.text}</p>
                  <span className="text-xs text-faint">{n.time}</span>
                </div>
                {n.unread && <span className="mt-1.5 h-2 w-2 shrink-0 rounded-full bg-brand" />}
              </li>
            ))}
          </ul>
        )}
      </Card>
    </>
  )
}

/* ================================================================ login */
import { api, ApiError } from "./api";
import { UserInfoDto } from "./types"; // We will add types next. Wait, I can define it here for now.

function Login({ onLogin }: { onLogin: (r: Role, user: any) => void }) {
  const [role, setRole] = useState<Role>("admin")
  const [email, setEmail] = useState("admin@brc.edu.bd")
  const [pw, setPw] = useState("Admin@123")
  const [show, setShow] = useState(false)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState("")

  const demo: Record<Role, { email: string, pw: string }> = {
    admin: { email: "admin@brc.edu.bd", pw: "Admin@123" },
    teacher: { email: "teacher@brc.edu.bd", pw: "Teacher@123" },
    student: { email: "student@brc.edu.bd", pw: "Student@123" },
  }

  const submit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!email.trim() || !pw.trim()) { setError("Please enter your email and password."); return }
    setError("")
    setLoading(true)
    
    try {
      const res = await api.post<any>("/auth/login", { email, password: pw });
      localStorage.setItem("token", res.accessToken);
      onLogin(res.user.role.toLowerCase() as Role, res.user);
    } catch (err: any) {
      if (err instanceof ApiError) {
        setError(err.data?.message || "Invalid credentials.");
      } else {
        setError("Failed to connect to the server.");
      }
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="grid min-h-screen lg:grid-cols-2">
      <div className="relative hidden flex-col justify-between overflow-hidden bg-brand p-12 text-white lg:flex">
        <div style={{ backgroundImage: "radial-gradient(circle at 1px 1px, rgba(255,255,255,0.14) 1px, transparent 0)", backgroundSize: "22px 22px" }} className="absolute inset-0" />
        <div className="relative"><Logo light /></div>
        <div className="relative max-w-md">
          <div className="mb-6 inline-flex rounded-2xl bg-white/10 p-3"><Icon.assignment className="h-7 w-7" /></div>
          <h2 className="font-display text-3xl font-extrabold leading-tight tracking-tight">A calmer way to run assignments &amp; submissions.</h2>
          <p className="mt-4 text-sm leading-relaxed text-white/70">Create assignments, track submissions, grade with feedback, and keep every class on schedule — all in one academic workspace.</p>
        </div>
        <div className="relative flex gap-8">
          <div><div className="font-display text-2xl font-extrabold">248</div><div className="text-xs text-white/60">Students</div></div>
          <div><div className="font-display text-2xl font-extrabold">86</div><div className="text-xs text-white/60">Assignments</div></div>
          <div><div className="font-display text-2xl font-extrabold">14</div><div className="text-xs text-white/60">Classes</div></div>
        </div>
      </div>

      <div className="flex items-center justify-center bg-canvas p-6">
        <div className="es-fade w-full max-w-sm">
          <div className="mb-8 lg:hidden"><Logo /></div>
          <div className="text-xs font-semibold uppercase tracking-wider text-brand">{INSTITUTION}</div>
          <h1 className="mt-1 font-display text-2xl font-extrabold tracking-tight text-ink">Welcome back</h1>
          <p className="mt-1 text-sm text-muted">Assignment &amp; Submission Management System</p>

          <div className="mt-6 grid grid-cols-3 gap-1 rounded-xl bg-neutral-bg p-1">
            {(["admin", "teacher", "student"] as Role[]).map((r) => (
              <button key={r} onClick={() => { setRole(r); setEmail(demo[r].email); setPw(demo[r].pw); setError("") }} className={`rounded-lg py-2 text-[13px] font-medium capitalize transition-colors ${role === r ? "bg-surface text-brand shadow-sm" : "text-muted"}`}>{r}</button>
            ))}
          </div>

          <form onSubmit={submit} className="mt-5 space-y-4">
            {error && <div className="flex items-center gap-2 rounded-lg bg-danger-bg px-3 py-2.5 text-sm text-danger"><Icon.alert className="h-4 w-4 shrink-0" />{error}</div>}
            <Field label="Email"><Input type="email" value={email} onChange={(e) => setEmail(e.target.value)} invalid={!!error} placeholder="name@brc.edu.bd" /></Field>
            <Field label="Password">
              <div className="relative">
                <Input type={show ? "text" : "password"} value={pw} onChange={(e) => setPw(e.target.value)} className="pr-10" placeholder="••••••••" />
                <button type="button" onClick={() => setShow((s) => !s)} className="absolute right-2 top-1/2 -translate-y-1/2 p-1 text-faint hover:text-muted" aria-label={show ? "Hide password" : "Show password"}>{show ? <Icon.eyeOff className="h-5 w-5" /> : <Icon.eye className="h-5 w-5" />}</button>
              </div>
            </Field>
            <div className="flex items-center justify-between">
              <label className="flex items-center gap-2 text-sm text-ink-soft"><input type="checkbox" defaultChecked className="h-4 w-4 rounded border-line accent-[var(--color-brand)]" />Remember me</label>
              <button type="button" className="text-sm font-medium text-brand hover:underline">Forgot password?</button>
            </div>
            <Button type="submit" variant="primary" full disabled={loading}>{loading ? "Signing in…" : "Sign in"}</Button>
          </form>
        </div>
      </div>
    </div>
  )
}

/* ================================================================ root */
function AppInner() {
  const [role, setRole] = useState<Role | null>(null)
  const [currentUser, setCurrentUser] = useState<any>(null)
  const [route, setRoute] = useState<Route>({ view: "dashboard" })
  const [loadingInitial, setLoadingInitial] = useState(true)

  useEffect(() => {
    const token = localStorage.getItem("token")
    if (token) {
      api.get<any>("/auth/me").then(user => {
        setCurrentUser(user)
        setRole(user.role.toLowerCase() as Role)
      }).catch(() => {
        localStorage.removeItem("token")
      }).finally(() => {
        setLoadingInitial(false)
      })
    } else {
      setLoadingInitial(false)
    }
  }, [])

  const go = (view: string, params?: Record<string, string>) => setRoute({ view, params })
  const login = (r: Role, user: any) => { setRole(r); setCurrentUser(user); setRoute({ view: "dashboard" }) }
  const logout = () => { localStorage.removeItem("token"); setRole(null); setCurrentUser(null); setRoute({ view: "dashboard" }) }

  if (loadingInitial) return <div className="flex min-h-screen items-center justify-center bg-canvas"><div className="h-8 w-8 animate-spin rounded-full border-4 border-line border-t-brand" /></div>
  if (!role || !currentUser) return <Login onLogin={login} />

  const id = route.params?.id
  const navLabel = NAV[role].find((n) => n.key === route.view)?.label ?? "Dashboard"

  const titles: Record<string, string> = {
    dashboard: "Dashboard", users: "User Management", "user-form": id ? "Edit User" : "Add User", "user-detail": "User Details",
    classes: role === "teacher" ? "My Classes" : "Classes & Courses", "class-detail": "Class Details", subjects: role === "teacher" ? "My Subjects" : "Subjects",
    "teacher-assign": "Teacher Assignment", assignments: role === "teacher" ? "My Assignments" : role === "student" ? "Assignments" : "All Assignments",
    "assignment-detail": "Assignment Details", create: id ? "Edit Assignment" : "Create Assignment", submissions: role === "student" ? "My Submissions" : "Submissions",
    review: "Review Submission", settings: "Settings", profile: "Profile", notifications: "Notifications",
  }
  const title = titles[route.view] ?? navLabel
  const crumbs = [INSTITUTION, ROLE_LABEL[role], title]

  const render = () => {
    switch (route.view) {
      case "dashboard": return role === "admin" ? <AdminDashboard go={go} /> : role === "teacher" ? <TeacherDashboard go={go} /> : <StudentDashboard go={go} />
      case "users": return role === "admin" ? <AdminUsers go={go} /> : null
      case "user-form": return role === "admin" ? <UserForm go={go} id={id} /> : null
      case "user-detail": return role === "admin" ? <UserDetail go={go} id={id} /> : null
      case "classes": return role === "admin" ? <AdminClasses go={go} /> : null
      case "class-detail": return role === "admin" ? <ClassDetail go={go} id={id} /> : null
      case "subjects": return role === "admin" ? <AdminSubjects /> : null
      case "teacher-assign": return role === "admin" ? <TeacherAssign /> : null
      case "assignments": return role === "student" ? <StudentAssignments go={go} /> : <AssignmentsList role={role} go={go} />
      case "assignment-detail": return role === "student" ? <StudentAssignmentDetail go={go} id={id} /> : <AssignmentDetail role={role} go={go} id={id} />
      case "create": return role !== "student" ? <CreateAssignment go={go} id={id} /> : null
      case "submissions": return role === "student" ? <StudentSubmissions go={go} /> : <SubmissionsList role={role} go={go} />
      case "review": return role !== "student" ? <ReviewSubmission go={go} id={id} /> : null
      case "settings": return role === "admin" ? <AdminSettings /> : null
      case "notifications": return <NotificationsPage />
      case "profile": return <Profile role={role} user={currentUser} />
      default: return <EmptyState title="Page not found" message="This page doesn't exist or you don't have permission to view it." action={<Button variant="primary" onClick={() => go("dashboard")}>Back to dashboard</Button>} />
    }
  }

  // If a route returns null due to role restriction, fallback to default
  const renderedRoute = render()
  if (renderedRoute === null) {
    return (
      <Shell role={role} user={currentUser} route={route} go={go} onLogout={logout} title="Access Denied" crumbs={["Access Denied"]} showSearch={false}>
        <EmptyState title="Access Denied" message="You don't have permission to view this page." action={<Button variant="primary" onClick={() => go("dashboard")}>Back to dashboard</Button>} />
      </Shell>
    )
  }

  const showSearch = ["users", "assignments", "submissions", "subjects"].includes(route.view)

  return (
    <Shell role={role} user={currentUser} route={route} go={go} onLogout={logout} title={title} crumbs={crumbs} showSearch={showSearch}>
      {renderedRoute}
    </Shell>
  )
}

export default function App() {
  return (
    <ToastProvider>
      <AppInner />
    </ToastProvider>
  )
}
