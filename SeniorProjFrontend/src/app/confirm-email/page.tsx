import dynamic from 'next/dynamic';

const ConfirmEmailClient = dynamic(() => import('@/components/confirm-email/ConfirmEmail'), { ssr: false });

export default function ConfirmEmail() {
    return <ConfirmEmailClient />;
}