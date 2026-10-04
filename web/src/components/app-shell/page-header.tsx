type PageHeaderProps = {
  titulo: string;
  descricao: string;
};

export function PageHeader({ titulo, descricao }: PageHeaderProps) {
  return (
    <div className="flex flex-col gap-1">
      <h1 className="font-heading text-2xl font-semibold">{titulo}</h1>
      <p className="text-muted-foreground">{descricao}</p>
    </div>
  );
}
