import type { Metadata } from "next";

import { PageHeader } from "@/components/app-shell/page-header";

export const metadata: Metadata = { title: "Kanban" };

export default function KanbanPage() {
  return <PageHeader titulo="Kanban" descricao="O kanban chega na Fatia 2." />;
}
