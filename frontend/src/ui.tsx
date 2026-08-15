import {
  createContext,
  useContext,
  useEffect,
  useState,
  type ReactNode,
} from "react"

/* ----------------------------------------------------------------- icons */
type IconProps = { className?: string }
const S = (d: ReactNode, extra?: object) => (p: IconProps) => (
  <svg
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    strokeWidth={1.7}
    strokeLinecap="round"
    strokeLinejoin="round"
    className={p.className ?? "h-5 w-5"}
    {...extra}
  >
    {d}
  </svg>
)

export const Icon = {
  dashboard: S(<><rect x="3" y="3" width="7" height="9" rx="1.5" /><rect x="14" y="3" width="7" height="5" rx="1.5" /><rect x="14" y="12" width="7" height="9" rx="1.5" /><rect x="3" y="16" width="7" height="5" rx="1.5" /></>),
  assignment: S(<><path d="M9 3h6a2 2 0 0 1 2 2v0H7v0a2 2 0 0 1 2-2Z" /><path d="M7 5H6a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7a2 2 0 0 0-2-2h-1" /><path d="M8.5 11.5h7M8.5 15h4.5" /></>),
  submission: S(<><path d="M12 3v10" /><path d="m8 9 4 4 4-4" /><path d="M5 15v3a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2v-3" /></>),
  classes: S(<><path d="m3 8 9-4 9 4-9 4-9-4Z" /><path d="M7 10v5c0 1 2.2 2.5 5 2.5s5-1.5 5-2.5v-5" /><path d="M21 8v5" /></>),
  subject: S(<><path d="M5 4h9a3 3 0 0 1 3 3v13H8a3 3 0 0 1-3-3V4Z" /><path d="M17 20h2V6a2 2 0 0 0-2-2" /><path d="M8.5 8.5H13M8.5 12H13" /></>),
  users: S(<><circle cx="9" cy="8" r="3.2" /><path d="M3.5 20a5.5 5.5 0 0 1 11 0" /><path d="M16 5.2a3 3 0 0 1 0 5.6" /><path d="M16.5 13.5a5.5 5.5 0 0 1 4 6.5" /></>),
  settings: S(<><circle cx="12" cy="12" r="3" /><path d="M19.4 15a1.6 1.6 0 0 0 .3 1.8l.1.1a2 2 0 1 1-2.8 2.8l-.1-.1a1.6 1.6 0 0 0-2.7 1.1V21a2 2 0 1 1-4 0v-.1A1.6 1.6 0 0 0 6.7 19.7l-.1.1a2 2 0 1 1-2.8-2.8l.1-.1A1.6 1.6 0 0 0 4 15H3.9a2 2 0 1 1 0-4H4a1.6 1.6 0 0 0 1-2.7l-.1-.1a2 2 0 1 1 2.8-2.8l.1.1A1.6 1.6 0 0 0 10.6 4V3.9a2 2 0 1 1 4 0V4a1.6 1.6 0 0 0 2.7 1l.1-.1a2 2 0 1 1 2.8 2.8l-.1.1A1.6 1.6 0 0 0 20 10.6H21a2 2 0 1 1 0 4h-.1a1.6 1.6 0 0 0-1.5 1Z" /></>),
  profile: S(<><circle cx="12" cy="8" r="3.5" /><path d="M5 20a7 7 0 0 1 14 0" /></>),
  plus: S(<path d="M12 5v14M5 12h14" />),
  search: S(<><circle cx="11" cy="11" r="7" /><path d="m20 20-3.2-3.2" /></>),
  bell: S(<><path d="M6 9a6 6 0 0 1 12 0c0 5 2 6 2 6H4s2-1 2-6Z" /><path d="M10 19a2 2 0 0 0 4 0" /></>),
  chevron: S(<path d="m9 6 6 6-6 6" />),
  chevronDown: S(<path d="m6 9 6 6 6-6" />),
  menu: S(<path d="M4 6h16M4 12h16M4 18h16" />),
  close: S(<path d="M6 6l12 12M18 6 6 18" />),
  eye: S(<><path d="M2 12s3.5-7 10-7 10 7 10 7-3.5 7-10 7-10-7-10-7Z" /><circle cx="12" cy="12" r="3" /></>),
  eyeOff: S(<><path d="M3 3l18 18" /><path d="M10.6 6.2A9.7 9.7 0 0 1 12 6c6.5 0 10 7 10 7a17 17 0 0 1-3.2 3.9M6.2 6.7A17 17 0 0 0 2 12s3.5 7 10 7a9.7 9.7 0 0 0 3.4-.6" /><path d="M9.5 9.5a3 3 0 0 0 4.2 4.2" /></>),
  dots: S(<><circle cx="5" cy="12" r="1.6" /><circle cx="12" cy="12" r="1.6" /><circle cx="19" cy="12" r="1.6" /></>),
  edit: S(<><path d="M4 20h4l10-10a2 2 0 0 0-4-4L4 16v4Z" /><path d="m13.5 6.5 4 4" /></>),
  trash: S(<><path d="M4 7h16M9 7V5a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v2M6 7l1 13a1 1 0 0 0 1 1h8a1 1 0 0 0 1-1l1-13" /></>),
  check: S(<path d="m5 12 5 5L20 7" />),
  checkCircle: S(<><circle cx="12" cy="12" r="9" /><path d="m8.5 12 2.3 2.3L15.5 9.5" /></>),
  clock: S(<><circle cx="12" cy="12" r="9" /><path d="M12 7v5l3 2" /></>),
  alert: S(<><path d="M12 3 2 20h20L12 3Z" /><path d="M12 10v4M12 17h.01" /></>),
  file: S(<><path d="M13 3H7a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V9l-6-6Z" /><path d="M13 3v6h6" /></>),
  upload: S(<><path d="M12 16V4m0 0-4 4m4-4 4 4" /><path d="M4 16v2a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2v-2" /></>),
  download: S(<><path d="M12 4v12m0 0 4-4m-4 4-4-4" /><path d="M4 18v0a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2" /></>),
  logout: S(<><path d="M15 4h3a2 2 0 0 1 2 2v12a2 2 0 0 1-2 2h-3" /><path d="M10 12H3m0 0 3.5-3.5M3 12l3.5 3.5" /></>),
  copy: S(<><rect x="9" y="9" width="11" height="11" rx="2" /><path d="M5 15V5a2 2 0 0 1 2-2h8" /></>),
  inbox: S(<><path d="M4 13h4l1.5 3h5L16 13h4" /><path d="M4 13 6 5h12l2 8v5a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2v-5Z" /></>),
  award: S(<><circle cx="12" cy="9" r="5" /><path d="m9 13-1.5 8L12 18l4.5 3L15 13" /></>),
  calendar: S(<><rect x="3" y="5" width="18" height="16" rx="2" /><path d="M3 9h18M8 3v4M16 3v4" /></>),
  refresh: S(<><path d="M4 12a8 8 0 0 1 14-5.3L20 8" /><path d="M20 4v4h-4" /><path d="M20 12a8 8 0 0 1-14 5.3L4 16" /><path d="M4 20v-4h4" /></>),
}

/* ----------------------------------------------------------------- button */
type BtnVariant = "primary" | "secondary" | "ghost" | "danger" | "subtle"
export function Button({
  children,
  variant = "secondary",
  size = "md",
  icon,
  full,
  className = "",
  ...rest
}: {
  children?: ReactNode
  variant?: BtnVariant
  size?: "sm" | "md"
  icon?: ReactNode
  full?: boolean
  className?: string
} & React.ButtonHTMLAttributes<HTMLButtonElement>) {
  const base =
    "inline-flex items-center justify-center gap-2 rounded-lg font-medium transition-all duration-150 disabled:opacity-50 disabled:pointer-events-none active:scale-[.98] whitespace-nowrap"
  const sizes = { sm: "h-8 px-3 text-[13px]", md: "h-10 px-4 text-sm" }
  const variants: Record<BtnVariant, string> = {
    primary: "bg-brand text-white hover:bg-brand-600 shadow-sm shadow-brand/20",
    secondary: "bg-surface text-ink-soft border border-line hover:bg-line-soft hover:border-line",
    ghost: "text-ink-soft hover:bg-line-soft",
    danger: "bg-danger text-white hover:brightness-110",
    subtle: "bg-brand-50 text-brand hover:brightness-[.97]",
  }
  return (
    <button className={`${base} ${sizes[size]} ${variants[variant]} ${full ? "w-full" : ""} ${className}`} {...rest}>
      {icon}
      {children}
    </button>
  )
}

export function IconButton({
  children,
  label,
  className = "",
  ...rest
}: { children: ReactNode; label: string; className?: string } & React.ButtonHTMLAttributes<HTMLButtonElement>) {
  return (
    <button
      aria-label={label}
      title={label}
      className={`inline-flex h-9 w-9 items-center justify-center rounded-lg text-muted hover:bg-line-soft hover:text-ink transition-colors ${className}`}
      {...rest}
    >
      {children}
    </button>
  )
}

/* ----------------------------------------------------------------- badge */
type Tone = "ok" | "warn" | "danger" | "info" | "neutral"
export function Badge({ tone = "neutral", children, dot }: { tone?: Tone; children: ReactNode; dot?: boolean }) {
  const map: Record<Tone, string> = {
    ok: "bg-ok-bg text-ok",
    warn: "bg-warn-bg text-warn",
    danger: "bg-danger-bg text-danger",
    info: "bg-info-bg text-info",
    neutral: "bg-neutral-bg text-ink-soft",
  }
  return (
    <span className={`inline-flex items-center gap-1.5 rounded-full px-2.5 py-1 text-xs font-medium ${map[tone]}`}>
      {dot && <span className="h-1.5 w-1.5 rounded-full bg-current" />}
      {children}
    </span>
  )
}

export function StatusBadge({ status }: { status: string }) {
  const toneFor: Record<string, Tone> = {
    Published: "ok", Active: "ok", Graded: "ok", Submitted: "info",
    Draft: "neutral", Inactive: "neutral", "Not Submitted": "neutral",
    Closed: "neutral", Pending: "warn", Late: "warn", Returned: "warn",
    Overdue: "danger", Suspended: "danger", Error: "danger",
  }
  return <Badge tone={toneFor[status] ?? "neutral"} dot>{status}</Badge>
}

/* ----------------------------------------------------------------- card */
export function Card({ children, className = "", pad = true }: { children: ReactNode; className?: string; pad?: boolean }) {
  return <div className={`rounded-xl border border-line bg-surface ${pad ? "p-5" : ""} ${className}`}>{children}</div>
}

export function SectionTitle({ children, action }: { children: ReactNode; action?: ReactNode }) {
  return (
    <div className="mb-4 flex items-center justify-between gap-3">
      <h2 className="font-display text-[15px] font-bold tracking-tight text-ink">{children}</h2>
      {action}
    </div>
  )
}

/* ----------------------------------------------------------------- inputs */
export function Field({ label, hint, error, required, children }: { label: string; hint?: string; error?: string; required?: boolean; children: ReactNode }) {
  return (
    <label className="block">
      <span className="mb-1.5 flex items-center gap-1 text-[13px] font-medium text-ink-soft">
        {label}
        {required && <span className="text-danger">*</span>}
      </span>
      {children}
      {error ? (
        <span className="mt-1.5 flex items-center gap-1 text-xs text-danger"><Icon.alert className="h-3.5 w-3.5" />{error}</span>
      ) : hint ? (
        <span className="mt-1.5 block text-xs text-muted">{hint}</span>
      ) : null}
    </label>
  )
}

const inputBase =
  "w-full rounded-lg border border-line bg-surface px-3 text-sm text-ink placeholder:text-faint transition-colors focus:border-brand-600 focus:ring-2 focus:ring-brand-600/15 focus:outline-none"

export function Input(props: React.InputHTMLAttributes<HTMLInputElement> & { invalid?: boolean }) {
  const { invalid, className = "", ...rest } = props
  return <input className={`${inputBase} h-10 ${invalid ? "border-danger focus:border-danger focus:ring-danger/15" : ""} ${className}`} {...rest} />
}

export function Textarea(props: React.TextareaHTMLAttributes<HTMLTextAreaElement>) {
  const { className = "", ...rest } = props
  return <textarea className={`${inputBase} min-h-[110px] resize-y py-2.5 leading-relaxed ${className}`} {...rest} />
}

export function Select({ children, className = "", ...rest }: React.SelectHTMLAttributes<HTMLSelectElement>) {
  return (
    <div className="relative">
      <select className={`${inputBase} h-10 appearance-none pr-9 ${className}`} {...rest}>{children}</select>
      <Icon.chevronDown className="pointer-events-none absolute right-2.5 top-1/2 h-4 w-4 -translate-y-1/2 text-faint" />
    </div>
  )
}

export function Combobox({
  value,
  onChange,
  options,
  placeholder,
  className = "",
}: {
  value: string
  onChange: (v: string) => void
  options: { value: string; label: string }[]
  placeholder?: string
  className?: string
}) {
  const [query, setQuery] = useState("")
  const [open, setOpen] = useState(false)
  
  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (!(event.target as HTMLElement).closest(".combo-box-wrapper")) {
        setOpen(false)
      }
    }
    document.addEventListener("mousedown", handleClickOutside)
    return () => document.removeEventListener("mousedown", handleClickOutside)
  }, [])

  const displayValue = open ? query : (options.find((o) => o.value === value)?.label || "")
  const filtered = options.filter(
    (o) => o.label.toLowerCase().includes(query.toLowerCase()) || o.value.toLowerCase().includes(query.toLowerCase())
  )

  return (
    <div className={`combo-box-wrapper relative ${className}`}>
      <input
        type="text"
        className={`${inputBase} h-10 pr-9`}
        value={displayValue}
        onChange={(e) => {
          setQuery(e.target.value)
          setOpen(true)
          if (!e.target.value) onChange("")
        }}
        onFocus={() => {
          setQuery("")
          setOpen(true)
        }}
        placeholder={placeholder}
      />
      <Icon.chevronDown className="pointer-events-none absolute right-2.5 top-1/2 h-4 w-4 -translate-y-1/2 text-faint" />
      {open && (
        <div className="absolute z-50 mt-1 max-h-60 w-full overflow-y-auto rounded-lg border border-line bg-surface py-1 shadow-lg shadow-black/5">
          {filtered.length === 0 ? (
            <div className="px-3 py-2 text-sm text-muted">No results found.</div>
          ) : (
            filtered.map((o) => (
              <button
                key={o.value}
                type="button"
                className="flex w-full items-center justify-between px-3 py-2 text-left text-sm text-ink transition-colors hover:bg-brand-50 hover:text-brand"
                onClick={() => {
                  onChange(o.value)
                  setOpen(false)
                  setQuery("")
                }}
              >
                <span className="truncate">{o.label}</span>
                {o.value === value && <Icon.check className="h-4 w-4 shrink-0 text-brand" />}
              </button>
            ))
          )}
        </div>
      )}
    </div>
  )
}

export function SearchInput({ className = "", ...rest }: React.InputHTMLAttributes<HTMLInputElement>) {
  return (
    <div className={`relative ${className}`}>
      <Icon.search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-faint" />
      <input className={`${inputBase} h-10 pl-9`} {...rest} />
    </div>
  )
}

/* ----------------------------------------------------------------- avatar */
const AV = ["#1e3a8a", "#0f766e", "#7c3a66", "#92400e", "#3730a3", "#155e63"]
export function Avatar({ name, size = 36 }: { name: string; size?: number }) {
  const initials = name.split(" ").filter((w) => !w.includes(".")).slice(0, 2).map((w) => w[0]).join("").toUpperCase()
  const color = AV[name.charCodeAt(0) % AV.length]
  return (
    <span
      className="inline-flex shrink-0 items-center justify-center rounded-full font-display font-semibold text-white"
      style={{ width: size, height: size, background: color, fontSize: size * 0.36 }}
    >
      {initials}
    </span>
  )
}

/* ----------------------------------------------------------------- table */
export function Table({ head, children }: { head: ReactNode; children: ReactNode }) {
  return (
    <div className="overflow-x-auto">
      <table className="w-full border-collapse text-sm">
        <thead>
          <tr className="border-b border-line text-left">{head}</tr>
        </thead>
        <tbody>{children}</tbody>
      </table>
    </div>
  )
}
export function Th({ children, className = "" }: { children?: ReactNode; className?: string }) {
  return <th className={`whitespace-nowrap px-4 py-3 text-xs font-semibold uppercase tracking-wide text-muted ${className}`}>{children}</th>
}
export function Td({ children, className = "" }: { children?: ReactNode; className?: string }) {
  return <td className={`px-4 py-3.5 align-middle text-ink-soft ${className}`}>{children}</td>
}
export function Tr({ children, onClick }: { children: ReactNode; onClick?: () => void }) {
  return (
    <tr onClick={onClick} className={`border-b border-line-soft last:border-0 ${onClick ? "cursor-pointer hover:bg-canvas/70 transition-colors" : ""}`}>
      {children}
    </tr>
  )
}

/* ----------------------------------------------------------------- pagination */
export function Pagination({ page, pages, onPage, total }: { page: number; pages: number; onPage: (p: number) => void; total: number }) {
  return (
    <div className="flex flex-col gap-3 border-t border-line px-4 py-3 sm:flex-row sm:items-center sm:justify-between">
      <span className="text-xs text-muted">Showing page {page} of {pages} · {total} results</span>
      <div className="flex items-center gap-1">
        <Button size="sm" variant="ghost" disabled={page <= 1} onClick={() => onPage(page - 1)}>Previous</Button>
        {Array.from({ length: pages }, (_, i) => i + 1).map((p) => (
          <button
            key={p}
            onClick={() => onPage(p)}
            className={`h-8 w-8 rounded-lg text-[13px] font-medium transition-colors ${p === page ? "bg-brand text-white" : "text-ink-soft hover:bg-line-soft"}`}
          >
            {p}
          </button>
        ))}
        <Button size="sm" variant="ghost" disabled={page >= pages} onClick={() => onPage(page + 1)}>Next</Button>
      </div>
    </div>
  )
}

/* ----------------------------------------------------------------- empty / loading / error */
export function EmptyState({ icon, title, message, action }: { icon?: ReactNode; title: string; message: string; action?: ReactNode }) {
  return (
    <div className="flex flex-col items-center justify-center px-6 py-16 text-center">
      <div className="mb-4 flex h-14 w-14 items-center justify-center rounded-2xl bg-brand-50 text-brand">{icon ?? <Icon.inbox />}</div>
      <h3 className="font-display text-base font-bold text-ink">{title}</h3>
      <p className="mt-1 max-w-sm text-sm text-muted">{message}</p>
      {action && <div className="mt-5">{action}</div>}
    </div>
  )
}

export function ErrorState({ onRetry, message }: { onRetry: () => void; message?: string }) {
  return (
    <div className="flex flex-col items-center justify-center px-6 py-16 text-center">
      <div className="mb-4 flex h-14 w-14 items-center justify-center rounded-2xl bg-danger-bg text-danger"><Icon.alert /></div>
      <h3 className="font-display text-base font-bold text-ink">Something went wrong</h3>
      <p className="mt-1 max-w-sm text-sm text-muted">{message ?? "Unable to load this content. Please try again."}</p>
      <Button className="mt-5" variant="secondary" icon={<Icon.refresh className="h-4 w-4" />} onClick={onRetry}>Retry</Button>
    </div>
  )
}

export function Skeleton({ className = "" }: { className?: string }) {
  return <div className={`es-skel rounded-md ${className}`} />
}

export function TableSkeleton({ rows = 5, cols = 5 }: { rows?: number; cols?: number }) {
  return (
    <div className="p-4">
      {Array.from({ length: rows }).map((_, r) => (
        <div key={r} className="flex items-center gap-4 border-b border-line-soft py-3.5 last:border-0">
          {Array.from({ length: cols }).map((_, c) => (
            <Skeleton key={c} className={`h-4 ${c === 0 ? "w-40" : "flex-1"}`} />
          ))}
        </div>
      ))}
    </div>
  )
}

/* ----------------------------------------------------------------- modal */
export function Modal({ open, onClose, title, children, footer, wide }: { open: boolean; onClose: () => void; title: string; children: ReactNode; footer?: ReactNode; wide?: boolean }) {
  useEffect(() => {
    if (!open) return
    const h = (e: KeyboardEvent) => e.key === "Escape" && onClose()
    window.addEventListener("keydown", h)
    return () => window.removeEventListener("keydown", h)
  }, [open, onClose])
  if (!open) return null
  return (
    <div className="fixed inset-0 z-50 flex items-end justify-center bg-ink/40 p-0 backdrop-blur-[2px] sm:items-center sm:p-4" onClick={onClose}>
      <div
        className={`es-fade w-full ${wide ? "max-w-2xl" : "max-w-md"} rounded-t-2xl border border-line bg-surface shadow-2xl sm:rounded-2xl`}
        onClick={(e) => e.stopPropagation()}
        role="dialog"
        aria-modal="true"
      >
        <div className="flex items-center justify-between border-b border-line px-5 py-4">
          <h3 className="font-display text-base font-bold text-ink">{title}</h3>
          <IconButton label="Close" onClick={onClose}><Icon.close className="h-5 w-5" /></IconButton>
        </div>
        <div className="max-h-[70vh] overflow-y-auto px-5 py-5">{children}</div>
        {footer && <div className="flex items-center justify-end gap-2 border-t border-line px-5 py-4">{footer}</div>}
      </div>
    </div>
  )
}

export function ConfirmDialog({ open, onClose, onConfirm, title, message, confirmLabel = "Confirm", danger }: {
  open: boolean; onClose: () => void; onConfirm: () => void; title: string; message: string; confirmLabel?: string; danger?: boolean
}) {
  return (
    <Modal
      open={open}
      onClose={onClose}
      title={title}
      footer={
        <>
          <Button variant="secondary" onClick={onClose}>Cancel</Button>
          <Button variant={danger ? "danger" : "primary"} onClick={() => { onConfirm(); onClose() }}>{confirmLabel}</Button>
        </>
      }
    >
      <div className="flex gap-3">
        <div className={`flex h-10 w-10 shrink-0 items-center justify-center rounded-full ${danger ? "bg-danger-bg text-danger" : "bg-brand-50 text-brand"}`}>
          <Icon.alert className="h-5 w-5" />
        </div>
        <p className="pt-1.5 text-sm text-ink-soft">{message}</p>
      </div>
    </Modal>
  )
}

/* ----------------------------------------------------------------- toast */
type Toast = { id: number; text: string; tone: Tone }
const ToastCtx = createContext<(text: string, tone?: Tone) => void>(() => {})
export const useToast = () => useContext(ToastCtx)

export function ToastProvider({ children }: { children: ReactNode }) {
  const [items, setItems] = useState<Toast[]>([])
  const push = (text: string, tone: Tone = "info") => {
    const id = Date.now() + Math.random()
    setItems((s) => [...s, { id, text, tone }])
    setTimeout(() => setItems((s) => s.filter((t) => t.id !== id)), 3400)
  }
  const iconFor: Record<Tone, ReactNode> = {
    ok: <Icon.checkCircle className="h-5 w-5 text-ok" />,
    info: <Icon.bell className="h-5 w-5 text-info" />,
    warn: <Icon.alert className="h-5 w-5 text-warn" />,
    danger: <Icon.alert className="h-5 w-5 text-danger" />,
    neutral: <Icon.bell className="h-5 w-5 text-muted" />,
  }
  return (
    <ToastCtx.Provider value={push}>
      {children}
      <div className="pointer-events-none fixed bottom-4 right-4 z-[60] flex w-[min(92vw,360px)] flex-col gap-2">
        {items.map((t) => (
          <div key={t.id} className="es-toast pointer-events-auto flex items-start gap-3 rounded-xl border border-line bg-surface px-4 py-3 shadow-lg">
            {iconFor[t.tone]}
            <p className="pt-0.5 text-sm text-ink-soft">{t.text}</p>
          </div>
        ))}
      </div>
    </ToastCtx.Provider>
  )
}

/* ----------------------------------------------------------------- tabs */
export function Tabs({ tabs, active, onChange }: { tabs: string[]; active: string; onChange: (t: string) => void }) {
  return (
    <div className="flex gap-1 overflow-x-auto border-b border-line">
      {tabs.map((t) => (
        <button
          key={t}
          onClick={() => onChange(t)}
          className={`relative whitespace-nowrap px-4 py-2.5 text-sm font-medium transition-colors ${active === t ? "text-brand" : "text-muted hover:text-ink-soft"}`}
        >
          {t}
          {active === t && <span className="absolute inset-x-3 -bottom-px h-0.5 rounded-full bg-brand" />}
        </button>
      ))}
    </div>
  )
}

/* ----------------------------------------------------------------- dropdown menu */
export function Menu({ trigger, children, align = "right" }: { trigger: ReactNode; children: (close: () => void) => ReactNode; align?: "left" | "right" }) {
  const [open, setOpen] = useState(false)
  useEffect(() => {
    if (!open) return
    const h = () => setOpen(false)
    window.addEventListener("click", h)
    return () => window.removeEventListener("click", h)
  }, [open])
  return (
    <div className="relative" onClick={(e) => e.stopPropagation()}>
      <div onClick={() => setOpen((o) => !o)}>{trigger}</div>
      {open && (
        <div className={`es-fade absolute z-40 mt-1 min-w-[180px] rounded-xl border border-line bg-surface p-1.5 shadow-xl ${align === "right" ? "right-0" : "left-0"}`}>
          {children(() => setOpen(false))}
        </div>
      )}
    </div>
  )
}
export function MenuItem({ children, onClick, danger, icon }: { children: ReactNode; onClick?: () => void; danger?: boolean; icon?: ReactNode }) {
  return (
    <button
      onClick={onClick}
      className={`flex w-full items-center gap-2.5 rounded-lg px-3 py-2 text-left text-sm transition-colors ${danger ? "text-danger hover:bg-danger-bg" : "text-ink-soft hover:bg-line-soft"}`}
    >
      {icon}
      {children}
    </button>
  )
}
