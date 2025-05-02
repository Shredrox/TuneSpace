import MainClientLayout from "@/layouts/client-layout";

export default function MainLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return <MainClientLayout>{children}</MainClientLayout>;
}
